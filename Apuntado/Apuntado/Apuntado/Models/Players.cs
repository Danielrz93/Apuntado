namespace Apuntado.Models
{
    using Apuntado.ViewModels;
    using global::SQLite;
    using System;

    [Table("Players")]
    public class Players : BaseViewModel
    {
        [PrimaryKey , AutoIncrement]
        public int IdPLayer { get; set; }

        public int IdGame { get; set; }
        
        [MaxLength(50)]
        public string Namep { get; set; }

        public int Points { get; set; }
    }
}
