using field_recording_api.Models.DataAccess;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace field_recording_api.DataAccess
{
    public class CoreRepository
    {
        private readonly ConnectionStringsModel _connectionStringsModel;
        private readonly DBHelpers _dBHelpers;

        public CoreRepository(DBHelpers dBHelpers, ConnectionStringsModel connectionStringsModel)
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

        //public DataTable callStoredSIIS(string spname, List<SqlParameter> listparam)
        //{
        //    try
        //    {
        //        return _dBHelpers.DBExecuteSP_ReturnDt(_connectionStringsModel.SIISConnection, spname, listparam);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public async Task<DataTable> callStoredAsync(string spname, List<SqlParameter> listparam)
        {
            try
            {
                return await _dBHelpers.DBExecuteSP_ReturnDt_async(_connectionStringsModel.FieldRecordingConnection, spname, listparam).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DataSet> callStoredDataSetAsync(string spname, List<SqlParameter> listparam)
        {
            try
            {
                return await _dBHelpers.DBExecuteSP_ReturnDs_async(_connectionStringsModel.FieldRecordingConnection, spname, listparam).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DataTable> callTablAsync(string sqlstring, List<SqlParameter> listparam = null)
        {
            try
            {
                return await _dBHelpers.DBExecuteTB_ReturnDt_async(_connectionStringsModel.FieldRecordingConnection, sqlstring, listparam).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> callUpdateTablAsync(string sqlstring, List<SqlParameter> listparam = null)
        {
            try
            {
                return await _dBHelpers.DBExecuteTB_Update_async(_connectionStringsModel.FieldRecordingConnection, sqlstring, listparam).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<dynamic> callInsertTableAsync(string spname, List<SqlParameter> listparam)
        {
            try
            {
                return await _dBHelpers.DBExecuteTB_Insert_async(_connectionStringsModel.FieldRecordingConnection, spname, listparam).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
