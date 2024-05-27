using field_recording_api.Models.AccountDetailModel;
using field_recording_api.Models.FileModel;
using field_recording_api.Models.HttpModel;

namespace field_recording_api.Services.AccountDetail
{
    public interface IAccountDetailServices
    {
        Task<ResponseContext> syncDataFile(CollecctionModel Req);
        Task<ResponseContext> getCollections(CollecctionModel Req);

        Task<ResponseContext> getCollectionsNew(CollecctionModel Req);
        Task<ResponseContext> searchCollectionsNew(SearchCollectionsModel Req);
        Task<ResponseContext> getAccountDetailDataNew(AccountDetailModel Req);


        Task<ResponseContext> searchCollections(SearchCollectionsModel Req);
        Task<ResponseContext> getAccountDetailData(AccountDetailModel Req);

        Task<ResponseContext> saveAccountDetailData(SaveAccountDetailModel Req);

        Task<ResponseContext> getLocationTracking(LocationTracking Req);

        Task<ResponseContext> AddLocationTracking(AddLocationTrackingReq Req);
    }
}
