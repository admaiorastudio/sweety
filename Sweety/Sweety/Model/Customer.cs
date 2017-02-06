namespace AdMaiora.Sweety.DTOs
{
    using System;
    using SQLite;


    public class Customer
    {
        [PrimaryKey]
        public int Id
        {
            get;
            set;
        }

        [Indexed]
        public string FirstName
        {
            get;
            set;
        }       

        [Indexed]
        public string LastName
        {
            get;
            set;
        }
    }
}
