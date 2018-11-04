namespace Apuntado.SQLite
{
    using global::SQLite;
    using Models;
    using System;
    using System.Threading.Tasks;

    public class Sqlite
    {

        #region Attributes
        SQLiteConnection con;
        private object models;
        #endregion

        #region Constructor
        public Sqlite(string dbPath)
        {
            this.con = new SQLiteConnection(dbPath);

        }

        public void CreateTable<T>()
        {
            con.CreateTable<T>();
            return;
        }

        public async Task<Response> GetAllTable<T>() where T: new()
        {
            try
            {                
                var result = con.Table<T>().ToList(); 
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok", 
                    Result = result,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Error",
                    Result = string.Format("Failed to retrieve data. {0}", ex.Message)
,
                };               
            }
        }

        public async Task<Response> AddNewReg( object data )
        {

            try
            {
                var result = con.Insert(data);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Registro creado con exito"
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = string.Format("Failed to retrieve data. {0}", ex.Message)
                };
            }
        }

        #endregion


    }
}
