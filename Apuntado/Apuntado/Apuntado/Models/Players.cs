namespace Apuntado.Models
{
    using global::SQLite;
    using System;

    [Table("Players")]
    public class Players
    {
        [PrimaryKey, AutoIncrement]
        public int IdGame { get; set; }

        [PrimaryKey, AutoIncrement]
        public int IdPlayer { get; set; }

        [MaxLength(50) , Unique]
        public string Namep { get; set; }
    }
}
