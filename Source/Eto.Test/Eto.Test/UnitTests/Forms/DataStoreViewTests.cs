﻿using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using NUnit.Framework;
using Eto.Forms;
using Eto;

namespace Eto.Test.UnitTests.Forms
{
	/// <summary>
	/// Unit tests for DataStoreView
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TestFixture]
	public class DataStoreViewTests
	{
		static int ItemCount { get { return GridViewUtils.ItemCount; } }
		DataStoreCollection model;
		DataStoreView d;

		[SetUp]
		public void Setup()
		{
			TestUtils.Invoke(() =>
			{
				model = GridViewUtils.CreateModel();
				d = new DataStoreView { Model = model };
			});
		}

		[Test]
		public void DataStoreView_WithNoFilterOrSort_ViewContainsAllModelItems()
		{
			TestUtils.Invoke(() =>
			{
				var view = d.View;
				Assert.AreEqual(ItemCount, view.Count);
				for (var i = 0; i < ItemCount; ++i)
					Assert.AreSame(model[i], view[i]);
			});
		}

		[Test]
		public void DataStoreView_WithOddItemFilter_ViewContainsOddModelItems()
		{
			TestUtils.Invoke(() =>
			{
				d.Filter = GridViewUtils.KeepOddItemsFilter;
				var view = d.View;
				Assert.AreEqual(ItemCount / 2, view.Count);
				for (var i = 0; i < ItemCount / 2; ++i)
					Assert.AreSame(model[i * 2 + 1], view[i]);
			});
		}

		[Test]
		public void DataStoreView_SortWithEvenItemsBeforeOddItems_SortsCorrectly()
		{
			TestUtils.Invoke(() =>
			{
				d.SortComparer = GridViewUtils.SortEvenItemsBeforeOdd;
				var view = d.View;
				Assert.AreEqual(ItemCount, view.Count);
				for (var i = 0; i < ItemCount / 2; ++i)
					Assert.AreSame(model[i * 2], view[i]);
				for (var i = 0; i < ItemCount / 2; ++i)
					Assert.AreSame(model[i * 2 + 1], view[ItemCount / 2 + i]);
			});
		}

		[Test]
		public void DataStoreView_SortAscending_SortsCorrectly()
		{
			TestUtils.Invoke(() =>
			{
				d.SortComparer = GridViewUtils.SortItemsAscending;
				var view = d.View;
				Assert.AreEqual(ItemCount, view.Count);
				for (var i = 0; i < ItemCount; ++i)
					Assert.AreSame(model[i], view[i]);
			});
		}

		[Test]
		public void DataStoreView_SortDescending_SortsCorrectly()
		{
			TestUtils.Invoke(() =>
			{
				d.SortComparer = GridViewUtils.SortItemsDescending;
				var view = d.View;
				Assert.AreEqual(ItemCount, view.Count);
				for (var i = 0; i < ItemCount; ++i)
					Assert.AreSame(model[ItemCount - 1 - i], view[i]);
			});
		}

		[Test]
		public void DataStoreView_SortWithEvenItemsBeforeOddItemsAndWithFilter_SortsAndFiltersCorrectly()
		{
			TestUtils.Invoke(() =>
			{
				d.SortComparer = GridViewUtils.SortEvenItemsBeforeOdd;
				d.Filter = GridViewUtils.KeepFirstHalfOfItemsFilter;
				var view = d.View;
				Assert.AreEqual(ItemCount / 2, view.Count);
				for (var i = 0; i < ItemCount / 4; ++i)
					Assert.AreSame(model[i * 2], view[i]);
				for (var i = 0; i < ItemCount / 4; ++i)
					Assert.AreSame(model[i * 2 + 1], view[ItemCount / 4 + i]);
			});
		}
	}
}