﻿using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace ProgrammingIdeas
{
    public class mAdapter : RecyclerView.Adapter
    {
        private List<Category> categories;

        public event EventHandler<int> ItemClick;

        private Context parentContext;
        private int[] icons;
        private int scrollPos;

        public mAdapter(List<Category> category, Context context, int[] icons, int scrollPos)
        {
            categories = category;
            parentContext = context;
            this.icons = icons;
            this.scrollPos = scrollPos;
        }

        public override int ItemCount
        {
            get
            {
                return categories.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var category = categories[position];

            var viewHolder = holder as mViewHolder;
            viewHolder.categoryLabel.Text = category.CategoryLbl;
            viewHolder.completionCount.Text = $"Ideas: {category.CategoryCount}";
            viewHolder.description.Text = category.Description;
            viewHolder.imageView.SetImageResource(icons[position]);
            viewHolder.Root.SetBackgroundColor(Android.Graphics.Color.Transparent);
            if (position == scrollPos)
                viewHolder.Root.SetBackgroundResource(Resource.Color.highlight);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.categoryrow, parent, false);
            return new mViewHolder(row, OnClick);
        }

        private void OnClick(int position)
        {
            if (ItemClick != null)
            {
                ItemClick?.Invoke(this, position);
            }
        }
    }

    public class mViewHolder : RecyclerView.ViewHolder
    {
        public ImageView imageView { get; set; }
        public TextView categoryLabel { get; set; }
        public TextView completionCount { get; set; }
        public TextView description { get; set; }
        public LinearLayout Root { get; set; }

        public mViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            View row = itemView;
            row.Click += (sender, e) => listener(AdapterPosition);
            imageView = itemView.FindViewById<ImageView>(Resource.Id.categoryIcon);
            categoryLabel = itemView.FindViewById<TextView>(Resource.Id.categoryLbl);
            completionCount = itemView.FindViewById<TextView>(Resource.Id.completedLbl);
            description = itemView.FindViewById<TextView>(Resource.Id.descriptionLbl);
            Root = itemView.FindViewById<LinearLayout>(Resource.Id.categoryRoot);
        }
    }
}