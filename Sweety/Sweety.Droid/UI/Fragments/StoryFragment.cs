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

    public class StoryFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes

        class StoryAdapter : ItemRecyclerAdapter<StoryAdapter.StoryViewHolder, Transaction>
        {
            #region Inner Classes

            public class StoryViewHolder : ItemViewHolder
            {
                [Widget]
                public View MarkerColorView;

                [Widget]
                public TextView TransactionValueLabel;

                [Widget]
                public TextView TransactionDetailsLabel;

                public StoryViewHolder(View itemView)
                    : base(itemView)
                {
                }
            }

            #endregion

            #region Costants and Fields
            #endregion

            #region Constructors

            public StoryAdapter(AdMaiora.AppKit.UI.App.Fragment context, IEnumerable<Transaction> source)
                : base(context, Resource.Layout.CellTransaction, source)
            {                
            }

            #endregion

            #region Public Methods

            public override void GetView(int postion, StoryViewHolder holder, View view, Transaction item)
            {
                holder.MarkerColorView.SetBackgroundColor(ViewBuilder.ColorFromARGB(item.MarkerColor));
                holder.TransactionValueLabel.Text = String.Format("Totale speso: {0:0.00} pts", item.Value);
                holder.TransactionDetailsLabel.Text = String.Format("consumeazione fatta @ {0:g}", item.ConsumptionDate);
            }

            #endregion

            #region Methods    
            #endregion
        }

        #endregion

        #region Constants and Fields

        private Transaction[] _transactions;

        private StoryAdapter _adapter;

        #endregion

        #region Widgets

        [Widget]
        private ItemRecyclerView StoryList;

        #endregion

        #region Constructors

        public StoryFragment()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Fragment Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _transactions = this.Arguments.GetObject<Transaction[]>("Transactions");
        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentStory, inflater, container);

            this.HasOptionsMenu = true;

            #endregion
                    
            LoadStory();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void LoadStory()
        {
            if (_transactions != null && _transactions.Length > 0)
            {
                _adapter = new StoryAdapter(this, _transactions);
                this.StoryList.SetAdapter(_adapter);
            }
        }

        #endregion

        #region Event Handlers
        #endregion
    }
}