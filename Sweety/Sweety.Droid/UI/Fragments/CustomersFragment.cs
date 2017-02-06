namespace AdMaiora.Sweety
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Util;
    using Android.Views;
    using Android.Widget;

    using AdMaiora.AppKit.UI;

    public class CustomersFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes

        class UserAdapter : ItemRecyclerAdapter<UserAdapter.UserViewHolder, User>
        {
            #region Inner Classes

            public class UserViewHolder : ItemViewHolder
            {
                [Widget]
                public ImageView UserImage;

                [Widget]
                public TextView UserNameLabel;

                [Widget]
                public TextView UserDetailsLabel;

                public UserViewHolder(View itemView)
                    : base(itemView)
                {
                }
            }

            #endregion

            #region Costants and Fields
            #endregion

            #region Constructors

            public UserAdapter(AdMaiora.AppKit.UI.App.Fragment context, IEnumerable<User> source)
                : base(context, Resource.Layout.CellUser, source)
            {                
            }

            #endregion

            #region Public Methods

            public override void GetView(int postion, UserViewHolder holder, View view, User item)
            {
                holder.UserNameLabel.Text = item.FullName;

                holder.UserDetailsLabel.Text = String.Format("valore card: {0:0.00} pts credito: {1:0.00} pts",
                    AppController.Globals.CardValue,
                    item.Card?.Credit ?? 0M);
            }

            #endregion

            #region Methods    
            #endregion
        }

        #endregion

        #region Constants and Fields

        private UserAdapter _adapter;

        #endregion

        #region Widgets

        [Widget]
        private ItemRecyclerView UserList;

        [Widget]
        private ImageButton CancelButton;

        #endregion

        #region Constructors

        public CustomersFragment()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Fragment Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentCustomers, inflater, container);

            this.HasOptionsMenu = true;

            #endregion
                    
            this.UserList.ItemSelected += UserList_ItemSelected;
            this.UserList.RequestFocus();

            LoadUsers();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            menu.Clear();
            menu.Add(0, 1, 0, "Aggiungi").SetShowAsAction(ShowAsAction.Always);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case 1:
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
            
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            this.UserList.ItemSelected -= UserList_ItemSelected;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void LoadUsers()
        {
            User[] users = new[]
            {
                new User() { FullName = "Francesco Colombo" },
                new User() { FullName = "Sara Zoggia" },
                new User() { FullName = "Giulia Zoggia" },
                new User() { FullName = "Nicola Marchesan" },
                new User() { FullName = "Dario Locci" },
                new User() { FullName = "Luke Skywalker" },
                new User() { FullName = "Darth Vader" },
                new User() { FullName = "Leia Organa" },
                new User() { FullName = "Obi Wan Kenobi" },
                new User() { FullName = "Han Solo" },
                new User() { FullName = "Lando Carlissian" }
            };

            _adapter = new UserAdapter(this, users);
            this.UserList.SetAdapter(_adapter);
        }

        #endregion

        #region Event Handlers

        private void UserList_ItemSelected(object sender, ItemListSelectEventArgs e)
        {
            User user = e.Item as User;

            var f = new CardFragment();
            f.Arguments = new Bundle();
            f.Arguments.PutObject<User>("User", user);
            this.FragmentManager.BeginTransaction()
                .AddToBackStack("BeforeCardFragment")
                .Replace(Resource.Id.ContentLayout, f, "CardFragment")
                .Commit();            
        }

        #endregion
    }
}