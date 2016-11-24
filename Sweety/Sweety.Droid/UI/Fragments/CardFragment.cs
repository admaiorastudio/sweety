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
    using Android.Support.V7.Widget;

    public class CardFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes

        class Marker
        {
            #region Properties

            public decimal Value
            {
                get;
                set;
            }

            public bool IsConsumed
            {
                get
                {
                    return this.Value == 0;
                }
            }

            #endregion
        }

        class CardAdapter : ItemRecyclerAdapter<CardAdapter.CardViewHolder, Marker>
        {
            #region Inner Classes

            public class CardViewHolder : ItemViewHolder
            {
                [Widget]
                public ImageView MarkerImage;

                [Widget]
                public TextView ValueLabel;

                public CardViewHolder(View itemView)
                    : base(itemView)
                {
                }
            }

            #endregion

            #region Costants and Fields
            #endregion

            #region Constructors

            public CardAdapter(AdMaiora.AppKit.UI.App.Fragment context, IEnumerable<Marker> source)
                : base(context, Resource.Layout.CellMarker, source)
            {
            }

            #endregion

            #region Public Methods

            public override void GetView(int postion, CardViewHolder holder, View view, Marker item)
            {                
                holder.MarkerImage.SetImageResource(
                    item.IsConsumed ? "button_marker_red" : (item.Value == 1 ? "button_marker_green" : "button_marker_yellow"));

                holder.ValueLabel.Text = item.Value.ToString();    
                //holder.ValueLabel.SetTextColor(
                //    item.IsConsumed ? ViewBuilder.ColorFromARGB("FFFFA399") : (item.Value == 1 ? ViewBuilder.ColorFromARGB("FF00D6C7") : ViewBuilder.ColorFromARGB("FFFFDD6A")));
            }

            #endregion

            #region Methods    
            #endregion
        }

        #endregion

        #region Constants and Fields

        private const decimal UserCredit = 15.45M;
        private const decimal MarkerValue = 1M;
        private const decimal MarkerStep = .05M;

        private CardAdapter _adapter;

        #endregion

        #region Widgets

        [Widget]
        private ItemRecyclerView MarkerList;

        #endregion

        #region Constructors

        public CardFragment()
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

            SetContentView(Resource.Layout.FragmentCard, inflater, container);

            #endregion

            var manager = new GridLayoutManager(this.Activity,
                5,
                GridLayoutManager.Vertical,
                false);

            this.MarkerList.HasFixedSize = true;
            this.MarkerList.SetLayoutManager(manager);            
            this.MarkerList.ItemSelected += MarkerList_ItemSelected;
            this.MarkerList.ItemLongPress += MarkerList_ItemLongPress;


            LoadCard();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void LoadCard()
        {
            List<Marker> markers = new List<Marker>();

            int maxMarkers = (int)Math.Round(CardFragment.UserCredit / CardFragment.MarkerValue);
            decimal credit = CardFragment.UserCredit;

            for (int i = 0; i < maxMarkers; i++)
            {
                markers.Add(new Marker { Value = CardFragment.MarkerValue });

                credit -= CardFragment.MarkerValue;
                if((int)Math.Truncate(credit) == 0)
                {
                    if(credit > 0)
                        markers.Add(new Marker { Value = credit });

                    break;
                }
            }

            _adapter = new CardAdapter(this, markers);
            this.MarkerList.SetAdapter(_adapter);           
        }

        #endregion

        #region Event Handlers

        private void MarkerList_ItemSelected(object sender, ItemListSelectEventArgs e)
        {
            Marker item = e.Item as Marker;

            if (item.Value >= CardFragment.MarkerStep)
                item.Value -= CardFragment.MarkerStep;

            this.MarkerList.ReloadData();
        }

        private void MarkerList_ItemLongPress(object sender, ItemListLongPressEventArgs e)
        {
            Marker item = e.Item as Marker;

            item.Value = item.Value > 0 ? 0 : CardFragment.MarkerValue;
            this.MarkerList.ReloadData();
        }


        #endregion
    }
}