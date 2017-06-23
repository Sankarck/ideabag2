using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using ProgrammingIdeas.Adapters;
using ProgrammingIdeas.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adapters;

namespace ProgrammingIdeas.Activities
{
    [Activity(Label = "Bookmarks", Theme = "@style/AppTheme")]
    public class BookmarksActivity : BaseActivity
    {
        private List<Category> allItems;
        private RecyclerView recyclerView;
        private LinearLayoutManager manager;
        private BookmarkListAdapter adapter;
        private List<Idea> bookmarksList = new List<Idea>();
        private string path, ideasdb = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ideasdb");
        private View emptyState;
        private ProgressBar progressBar;

        public override int LayoutResource => Resource.Layout.bookmarksactivity;

        public override bool HomeAsUpEnabled => true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            path = Path.Combine(Global.APP_PATH, "bookmarks.json");
            recyclerView = FindViewById<RecyclerView>(Resource.Id.bookmarkRecyclerView);
            emptyState = FindViewById(Resource.Id.empty);
            progressBar = FindViewById<ProgressBar>(Resource.Id.completedIdeasBar);
            manager = new LinearLayoutManager(this);
            SetupUI();
        }

        private async void SetupUI()
        {
            if (File.Exists(path))
            {
                bookmarksList = await DBAssist.DeserializeDBAsync<List<Idea>>(path);
                bookmarksList = bookmarksList ?? new List<Idea>();
                allItems = Global.Categories;
                if (bookmarksList.Count > 0)
                {
                    adapter = new BookmarkListAdapter(bookmarksList);
                    adapter.ItemClick += OnItemClick;
                    recyclerView.SetAdapter(adapter);
                    recyclerView.SetLayoutManager(manager);
                    recyclerView.SetItemAnimator(new DefaultItemAnimator());
                    manager.ScrollToPosition(Global.BookmarkScrollPosition);
                    adapter.StateClicked += StateClicked;
                    ShowProgress();
                }
                else
                    ShowEmptyState();
            }
            else
                ShowEmptyState();
        }

        protected override void OnDestroy()
        {
            adapter.StateClicked -= StateClicked;
            adapter.ItemClick -= OnItemClick;
            base.OnDestroy();
        }

        private void ShowProgress()
        {
            var completedCount = bookmarksList.FindAll(x => x.State == Status.Done).Count;
            progressBar.Max = bookmarksList.Count;
            progressBar.Progress = 0;
            progressBar.IncrementProgressBy(completedCount);
        }

        private void ShowEmptyState()
        {
            recyclerView.Visibility = ViewStates.Gone;
            emptyState.Visibility = ViewStates.Visible;
            emptyState.FindViewById<TextView>(Resource.Id.infoText).Text += " bookmarks.";
        }

        private void StateClicked(string e)
        {
            var contents = e.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int position = Convert.ToInt32(contents[0]);
            string state = contents[1];
            if (bookmarksList != null && bookmarksList.Count != 0)
            {
                bookmarksList[position].State = state;
                adapter.NotifyDataSetChanged();
                allItems.FirstOrDefault(x => x.CategoryLbl == bookmarksList[position].Category).Items[position].State = state;
                DBAssist.SerializeDBAsync(path, bookmarksList);
            }

            Snackbar.Make(recyclerView, $"Idea progress marked as {state}.", Snackbar.LengthLong).Show();
            ShowProgress();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    NavigateAway();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnPause()
        {
            if (ideasdb != null)
                DBAssist.SerializeDBAsync(ideasdb, allItems);
            base.OnPause();
        }

        private void OnItemClick(int position)
        {
            Global.BookmarkScrollPosition = position;
            StartActivity(new Intent(this, typeof(BookmarkDetailsActivity)));
            OverridePendingTransition(Resource.Animation.push_left_in, Resource.Animation.push_left_out);
        }
    }
}