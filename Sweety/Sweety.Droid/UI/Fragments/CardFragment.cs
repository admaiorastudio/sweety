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

            private int _assignedColorId = 0;

            private string[] _colors = new[]
            {
                "550527", "688E26", "FAA613", "F44708", "A10702", "074F57", "077187", "74A57F",
                "9ECE9A", "E4C5AF", "7B7263", "C6CA53", "C9DCB3", "35CE8D", "BCD8B7", "95B8D1",
                "A663CC", "B298DC", "B8D0EB", "1C3A13", "6F2DBD", "BF6900", "C2E1C2", "AD6A6C"
            };

            private Dictionary<int, string> _assignedColors;

            #endregion

            #region Constructors

            public CardAdapter(AdMaiora.AppKit.UI.App.Fragment context, IEnumerable<Marker> source)
                : base(context, Resource.Layout.CellMarker, source)
            {
                _assignedColors = new Dictionary<int, string>();
            }

            #endregion

            #region Public Methods

            public override void GetView(int postion, CardViewHolder holder, View view, Marker item)
            {                
                string markerImage = "button_marker_empty";
                if(item.IsUsed)
                {
                    if (!item.IsChange)
                    {
                        if (item.IsConsumed)
                            markerImage = "button_marker_green";
                        else if (item.IsPartial)
                            markerImage = "button_marker_yellow";
                    }
                    else
                    {                        
                        markerImage = "button_marker_gray";
                    }
                }

                if(!String.IsNullOrWhiteSpace(item.TransactionColor))
                    view.SetBackgroundColor(ViewBuilder.ColorFromARGB(item.TransactionColor));

                holder.MarkerImage.SetImageResource(markerImage);    
                                
                holder.ValueLabel.Text = item.Value < AppController.Globals.MarkerValue ? item.Value.ToString().Substring(1) : ((int)(item.Value)).ToString();
                holder.ValueLabel.Visibility = item.IsUsed ? ViewStates.Visible : ViewStates.Gone;
            }

            public void CommitTransaction(int transactionId, decimal value)
            {
                if (value <= 0)
                    return;

                if (!_assignedColors.ContainsKey(transactionId))
                {
                    if (_assignedColorId == _colors.Length)
                        _assignedColorId = 0;

                    _assignedColors.Add(transactionId, _colors[_assignedColorId++]);
                }

                Marker marker = null;
                int maxMarkers = (int)Math.Round(value / AppController.Globals.MarkerValue);
                for (int i = 0; i < maxMarkers; i++)
                {
                    marker = this.SourceItems.FirstOrDefault(x => x.TransactionId == 0 && !x.IsUsed && !x.IsChange);
                    marker.TransactionId = transactionId;
                    marker.TransactionColor = _assignedColors[transactionId];
                    marker.Value = AppController.Globals.MarkerValue;

                    value -= AppController.Globals.MarkerValue;
                    if ((int)Math.Truncate(value) == 0)
                    {
                        if (value > 0)
                        {
                            marker = this.SourceItems.FirstOrDefault(x => x.TransactionId == 0 && !x.IsUsed && !x.IsChange);
                            marker.TransactionId = transactionId;
                            marker.TransactionColor = _assignedColors[transactionId];
                            marker.Value = value;
                        }

                        break;
                    }
                }
            }

            public void RevertTransaction(int transactionId)
            {
                this.SourceItems
                    .Where(x => x.TransactionId == transactionId)
                    .ToList()
                    .ForEach(x =>
                    {
                        _assignedColors.Remove(transactionId);

                        x.TransactionId = 0;
                        x.TransactionColor = null;
                        x.Value = 0;

                    });
            }


            public decimal GetValueForTransaction(int transactionId)
            {
                return this.SourceItems
                    .Where(x => x.TransactionId == transactionId)
                    .Sum(x => x.Value);
            }

            public decimal GetChangeForTransaction(int transactionId)
            {
                return this.SourceItems
                    .Where(x => x.TransactionId == transactionId)                    
                    .Where(x => x.IsPartial)
                    .Sum(x => AppController.Globals.MarkerValue - x.Value);
            }

            public string GetColorForTransaction(int transactionId)
            {
                var item = this.SourceItems
                    .FirstOrDefault(x => x.TransactionId == transactionId && !String.IsNullOrWhiteSpace(x.TransactionColor));

                return item?.TransactionColor;
            }

            public void RemoveChangeMarker()
            {                
                this.SourceItems.RemoveAll(x => x.IsChange);
            }

            #endregion

            #region Methods    
            #endregion
        }

        #endregion

        #region Constants and Fields

        private User _user;

        private CardAdapter _adapter;

        private bool _isInTransaction;

        private IMenu _menu;

        private int _newTransactionId;
        private Transaction _transaction;        

        #endregion

        #region Widgets

        [Widget]
        private RelativeLayout HeaderLayout;
       
        [Widget]
        private TextView UserNameLabel;

        [Widget]
        private TextView CardValueLabel;

        [Widget]
        private TextView CreditValueLabel;

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

            _user = this.Arguments.GetObject<User>("User");

        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentCard, inflater, container);

            this.HasOptionsMenu = true;

            #endregion

            this.HeaderLayout.Clickable = true;
            this.HeaderLayout.SetOnTouchListener(GestureListener.ForSingleTapUp(this.Activity,
                (e) =>
                {
                    var f = new StoryFragment();
                    f.Arguments = new Bundle();
                    f.Arguments.PutObject<Transaction[]>("Transactions", _user.Card.Transactions?.ToArray());
                    this.FragmentManager.BeginTransaction()
                        .AddToBackStack("BeforeStoryFragment")
                        .Replace(Resource.Id.ContentLayout, f, "StoryFragment")
                        .Commit();
                }));            
           
            var manager = new GridLayoutManager(this.Activity,
                5,
                GridLayoutManager.Vertical,
                false);

            this.MarkerList.HasFixedSize = true;
            this.MarkerList.SetLayoutManager(manager);            
            this.MarkerList.ItemSelected += MarkerList_ItemSelected;
            this.MarkerList.ItemLongPress += MarkerList_ItemLongPress;
            this.MarkerList.Enabled = false;


            LoadCard();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            _menu = menu;

            menu.Clear();
            menu.Add(0, 1, 0, "Consuma").SetShowAsAction(ShowAsAction.Always);                        
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case 1:
                    BeginTransaction();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }            
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            this.MarkerList.ItemSelected -= MarkerList_ItemSelected;
            this.MarkerList.ItemLongPress -= MarkerList_ItemLongPress;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void LoadCard(bool createNew = false)
        {
            if (_user.Card == null || createNew)
            {
                _user.Card = new Card();
                _user.Card.Value = AppController.Globals.CardValue;
                _user.Card.Credit = AppController.Globals.CardValue;

                List<Marker> markers = new List<Marker>();
                int maxMarkers = (int)Math.Round(_user.Card.Credit / AppController.Globals.MarkerValue);
                decimal credit = _user.Card.Value;
                for (int i = 0; i < maxMarkers; i++)
                {
                    markers.Add(new Marker { Value = 0 });

                    credit -= AppController.Globals.MarkerValue;
                    if ((int)Math.Truncate(credit) == 0)
                    {
                        if (credit > 0)
                            markers.Add(new Marker { Value = 0 });

                        break;
                    }
                }

                _user.Card.Markers = markers.ToArray();
                _user.Card.Transactions = null;
            }

            this.UserNameLabel.Text = _user.FullName;

            this.CardValueLabel.Text = String.Format("valore card: {0:0.00}", AppController.Globals.CardValue);

            this.CreditValueLabel.Text = _user.Card.Credit.ToString("0.00");
            
            _adapter = new CardAdapter(this, _user.Card.Markers);
            this.MarkerList.SetAdapter(_adapter);           
        }

        private void BeginTransaction()
        {
            if (_isInTransaction)
                return;

            _isInTransaction = true;

            _menu.FindItem(1).SetVisible(!_isInTransaction);            

            _transaction = new Transaction();
            _transaction.TransactionId = ++_newTransactionId;            
            _transaction.Value = 0;

            this.HeaderLayout.Clickable = false;

            var f = new ValuePickerWidget();
            f.MaxValue = _user.Card.Credit;
            f.ValueChanged += ValuePickerWidget_ValueChanged;
            this.FragmentManager.BeginTransaction()
                .AddToBackStack("BeforeValuePickerWidget")
                .Add(Resource.Id.ContentLayout, f, "ValuePickerFragment")
                .Commit();
        }

        private void EndTransaction(decimal value)
        {
            _isInTransaction = false;

            _menu.FindItem(1).SetVisible(!_isInTransaction);
            
            if (value == 0)
            {
                _adapter.RevertTransaction(_transaction.TransactionId);

                _transaction = null;

                this.HeaderLayout.Clickable = true;
            }
            else
            {
                _adapter.CommitTransaction(_transaction.TransactionId, value);

                _transaction.Value = _adapter.GetValueForTransaction(_transaction.TransactionId);
                _transaction.MarkerColor = _adapter.GetColorForTransaction(_transaction.TransactionId);
                _transaction.ConsumptionDate = DateTime.Now;

                _user.Card.Credit -= _transaction.Value;

                var transactions = new List<Transaction>(_user.Card.Transactions ?? new Transaction[0]);
                transactions.Add(_transaction);
                _user.Card.Transactions = transactions.ToArray();

                ProcessChange();

                _transaction = null;

                this.HeaderLayout.Clickable = true;

                this.CreditValueLabel.Text = _user.Card.Credit.ToString("0.00");
            }

            this.MarkerList.ReloadData();
        }

        private void ProcessChange()
        {
            _adapter.RemoveChangeMarker();

            decimal change = _user.Card.Change;
            change += _adapter.GetChangeForTransaction(_transaction.TransactionId);
            
            if (change == 0)
                return;

            if (change < AppController.Globals.MarkerValue)
            {
                _adapter.AddItem(new Marker { Value = change, IsChange = true });
            }
            else
            {
                int maxMarkers = (int)Math.Round(change / AppController.Globals.MarkerValue);
                for (int i = 0; i < maxMarkers; i++)
                {
                    _adapter.AddItem(new Marker { Value = 0 });

                    change -= AppController.Globals.MarkerValue;                    
                    if ((int)Math.Truncate(change) == 0)
                    {
                        if (change > 0)
                            _adapter.AddItem(new Marker { Value = change, IsChange = true });

                        break;
                    }
                }
            }

            _user.Card.Change = change;
        }

        #endregion

        #region Event Handlers

        private void MarkerList_ItemSelected(object sender, ItemListSelectEventArgs e)
        {
        }

        private void MarkerList_ItemLongPress(object sender, ItemListLongPressEventArgs e)
        {
        }

        private void ValuePickerWidget_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            EndTransaction(e.Value);
        }

        #endregion
    }
}