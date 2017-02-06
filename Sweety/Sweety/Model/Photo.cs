namespace AdMaiora.Sweety.DTOs
{
    using System;
    using SQLite;

    public class Photo
    {
        [PrimaryKey]
        public int Id
        {
            get;
            set;
        }


        [Indexed]
        public string Name
        {
            get;
            set;
        }       

        public string Description
        {
            get;
            set;
        }

        public string Data
        {
            get;
            set;
        }
    }
}