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

    public class ValueChangedEventArgs
    {
        public decimal Value
        {
            get;
            private set;
        }

        public ValueChangedEventArgs(decimal value)
        {
            this.Value = value;
        }
    }

    public class ValuePickerWidget : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        private decimal _totalValue;

        #endregion

        #region Events

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        #endregion

        #region Widgets

        [Widget]
        private TextView TotalValueLabel;

        [Widget]
        private TextView MaxValueLabel;

        [Widget]
        private Button Value1Button;

        [Widget]
        private Button Value01Button;

        [Widget]
        private Button Value05Button;

        [Widget]
        private Button Value10Button;

        [Widget]
        private Button Value20Button;

        [Widget]
        private Button Value50Button;

        [Widget]
        private ImageButton OkButton;

        private Button[] Buttons;

        #endregion

        #region Constructors

        public ValuePickerWidget()
        {
        }

        #endregion

        #region Properties

        public decimal MaxValue
        {
            get
            {
                return (decimal)this.Arguments.GetFloat("MaxValue");
            }
            set
            {
                this.Arguments.PutFloat("MaxValue", (float)value);
            }
        }

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

            SetContentView(Resource.Layout.WidgetValuePicker, inflater, container);

            #endregion

            this.MaxValueLabel.Text = String.Format("(massimo consumabile: {0:0.00} pts)", this.MaxValue);

            this.Buttons = new[]
            {
                this.Value1Button,
                this.Value01Button,
                this.Value05Button,
                this.Value10Button,
                this.Value20Button,
                this.Value50Button
            };           

            this.Value1Button.Click += ValueButton_Click;
            this.Value01Button.Click += ValueButton_Click;
            this.Value05Button.Click += ValueButton_Click;
            this.Value10Button.Click += ValueButton_Click;
            this.Value20Button.Click += ValueButton_Click;
            this.Value50Button.Click += ValueButton_Click;

            this.OkButton.Click += OkButton_Click;
        }

        public override bool OnBackButton()
        {
            OnValueChanged(0);

            return base.OnBackButton();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            this.Value1Button.Click -= ValueButton_Click;
            this.Value01Button.Click -= ValueButton_Click;
            this.Value05Button.Click -= ValueButton_Click;
            this.Value10Button.Click -= ValueButton_Click;
            this.Value20Button.Click -= ValueButton_Click;
            this.Value50Button.Click -= ValueButton_Click;

            this.OkButton.Click -= OkButton_Click;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Event Raising Methods

        protected void OnValueChanged(decimal value)
        {
            this.ValueChanged?.Invoke(this, new ValueChangedEventArgs(value));
        }

        #endregion

        #region Methods
        #endregion

        #region Event Handlers

        private void ValueButton_Click(object sender, EventArgs e)
        {
            if(_totalValue >= this.MaxValue
                && this.MaxValue != 0)
            {
                Toast.MakeText(this.Activity.Application, "Impossibile superare il consumo massimo!", ToastLength.Long).Show();
                return;
            }

            decimal[] values = { 1M, .01M, .05M, .10M, .20M, .50M };
            int index = Array.IndexOf(this.Buttons, sender);

            _totalValue += values[index];
            this.TotalValueLabel.Text = String.Format("{0:0.00} pts", _totalValue);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            OnValueChanged(_totalValue);

            this.FragmentManager.PopBackStack();
        }

        #endregion
    }
}