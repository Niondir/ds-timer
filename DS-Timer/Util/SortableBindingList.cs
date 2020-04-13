﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace DS_Timer.Util
{
	public class SortableBindingList<T> : BindingList<T>
	{

		// reference to the list provided at the time of instantiation
		List<T> originalList;
		ListSortDirection sortDirection;
		PropertyDescriptor sortProperty;

		// function that refereshes the contents
		// of the base classes collection of elements
		Action<SortableBindingList<T>, List<T>>
					   populateBaseList = (a, b) => a.ResetItems(b);


		/// <summary>
		/// Compares two objects based on a specific propery value
		/// </summary>
		class PropertyCompare : IComparer<T>
		{
			PropertyDescriptor _property;           // The propery value
			ListSortDirection _direction;           // The direction of compare ascendinf or descending

			public PropertyCompare(PropertyDescriptor property, ListSortDirection direction)
			{
				_property = property;
				_direction = direction;
			}

			public int Compare(T comp1, T comp2)
			{
				var value1 = _property.GetValue(comp1) as IComparable;
				var value2 = _property.GetValue(comp2) as IComparable;

                
                

				if (_direction == ListSortDirection.Ascending)
				{
                    if (!(value1 is IComparable)) return 0;
					return value1.CompareTo(value2);
				}
				else
				{
                    if (!(value2 is IComparable)) return 0;
					return value2.CompareTo(value1);
				}
			}
		}

		public SortableBindingList()
		{
			originalList = new List<T>();
		}

		public SortableBindingList(IEnumerable<T> enumerable)
		{
			originalList = new List<T>(enumerable);
			populateBaseList(this, originalList);
		}

		public SortableBindingList(List<T> list)
		{
			originalList = list;
			populateBaseList(this, originalList);
		}

		protected override void ApplySortCore(PropertyDescriptor prop,
								ListSortDirection direction)
		{
			//sortProperty = prop;
			//sortDirection = direction;

			PropertyCompare comp = new PropertyCompare(prop, sortDirection);
			List<T> sortedList = new List<T>(this);
			try
			{
				sortedList.Sort(comp);
			}
			catch(Exception) {
				// What if not sortable?
			}
			ResetItems(sortedList);

			ResetBindings();
			sortDirection = sortDirection == ListSortDirection.Ascending ?
							ListSortDirection.Descending : ListSortDirection.Ascending;
		}


		protected override void RemoveSortCore()
		{
			ResetItems(originalList);
		}

		private void ResetItems(List<T> items)
		{

			base.ClearItems();

			for (int i = 0; i < items.Count; i++)
			{
				base.InsertItem(i, items[i]);
			}
		}

		protected override bool SupportsSortingCore
		{
			get
			{
				// indeed we do
				return true;
			}
		}

		protected override ListSortDirection SortDirectionCore
		{
			get
			{
				return sortDirection;
			}
		}

		protected override PropertyDescriptor SortPropertyCore
		{
			get
			{
				return sortProperty;
			}
		}

	
		protected override void OnListChanged(ListChangedEventArgs e)
		{
			base.OnListChanged(e);
			originalList = new List<T>(base.Items);
		}

		/*
		protected override void OnListChanged(ListChangedEventArgs e)
		{
			originalList = base.Items.ToList();
		}*/
	}
}
