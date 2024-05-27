using field_recording_api.Models.DataAccess;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace field_recording_api.DataAccess
{
    public class CollectionRepository
    {
        private readonly ConnectionStringsModel _connectionStringsModel;
        private readonly DBHelpers _dBHelpers;

        public CollectionRepository(DBHelpers dBHelpers, ConnectionStringsModel connectionStringsModel)
        {
            _dBHelpers = dBHelpers;
            _connectionStringsModel = connectionStringsModel;
        }

        public DataTable callStored(string spname, List<SqlParameter> listparam)
        {
            try
            {
                return _dBHelpers.DBExecuteSP_ReturnDt(_connectionStringsModel.FieldRecordingConnection, spname, listparam);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
