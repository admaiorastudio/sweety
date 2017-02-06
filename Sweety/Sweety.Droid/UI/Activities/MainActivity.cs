namespace AdMaiora.Sweety
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Content.Res;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;

    using AdMaiora.AppKit.UI;

    [Activity(
        Label = "Sweety",
        MainLauncher = true,
        Theme = "@style/AppTheme",
        LaunchMode = LaunchMode.SingleTask,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges =
            ConfigChanges.Orientation | ConfigChanges.ScreenSize |
            ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard
    )]
    public class MainActivity : AdMaiora.AppKit.UI.App.AppCompactActivity
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields
        #endregion

        #region Widgets

        [Widget]
        private FrameLayout LoadLayout;

        #endregion

        #region Constructors

        public MainActivity()
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Activity Methods

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            #region Desinger Stuff

            SetContentView(Resource.Layout.ActivityMain, Resource.Id.ContentLayout, Resource.Id.Toolbar);

            this.SupportActionBar.SetDisplayShowHomeEnabled(true);
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            #endregion

            this.LoadLayout.Focusable = true;
            this.LoadLayout.FocusableInTouchMode = true;
            this.LoadLayout.Clickable = true;
            this.LoadLayout.Visibility = ViewStates.Gone;

            bool isResuming = this.SupportFragmentManager.FindFragmentById(Resource.Id.ContentLayout) != null;
            if (!isResuming)
            {
                var f = new CustomersFragment();
                this.SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.ContentLayout, f, "UserFragment")
                    .Commit();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Block the main UI, preventing user from tapping any view
        /// </summary>
        public void BlockUI()
        {
            if (this.LoadLayout != null)
                this.LoadLayout.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Unblock the main UI, allowing user tapping views
        /// </summary>
        public void UnblockUI()
        {
            if (this.LoadLayout != null)
                this.LoadLayout.Visibility = ViewStates.Gone;
        }

        #endregion

        #region Methods
        #endregion

        #region Event Handlers
        #endregion
    }
}