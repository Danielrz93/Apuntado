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
        #endregion

        #region Constructor
        public Sqlite(string dbPath)
        {
            this.con = new SQLiteConnection(dbPath);

        }

        public void Commit_or_Rollback(string action)
        {
            if (action == "Commit")
            {
                con.Commit();
            }
            else if (action == "Rollback")
            {
                con.Rollback();
            }            
        }

        public void CreateTable<T>()
        {
            con.CreateTable<T>();
            return;
        }

        public void DeleteTable<T>()
        {
            con.DeleteAll<T>();
            con.DropTable<T>();
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

        public async Task<Response> DeleteReg<T>(object data)
        {

            try
            {
                var result = con.Delete<T>(data);                
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

        public async Task<Response> DeleteReg(object data)
        {

            try
            {
                var result = con.Delete(data);
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

        public async Task<Response> GetQuery<T>(string query) where T: new()
        {
            try
            {
                var result = con.Query<T>(query);

                if (result != null)
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Result = result
                    };
                }
                else
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No se encontrarón registros"
                    };
                }
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
        
        public async Task<Response> Update<T>(object data)
        {
            try
            {
                var result = con.Update(data);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Resgistro actualizado con exito"
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
