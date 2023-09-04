using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Poc_GetProvinceRegionWithChapterId : IEntityProcView
    {
        public const string ProcName = "Poc_GetProvinceRegionWithChapterId";

        [Key]
        public int RegionId { get; set; }
        public int ParticipatingProvinceId { get; set; }
        public string RegionName { get; set; }
        public string ParticipatingProvinceName { get; set; }


        public Poc_GetProvinceRegionWithChapterId()
		{
		}

        public static IEntityProc GetEntityProc(int chapterId)
        {
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);
            return new EntityProc(
                $"{ProcName} @ChapterId",
                new SqlParameter[] {
                    ChapterId
                }
            );
        }
    }
}
