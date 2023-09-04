using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Chapter
{
    public class ChapterViewModelCreate : IEntityBase
    {
        public ChapterViewModelCreate()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public string Note { get; set; }
        public int RegionId { get; set; }
        public int ProvinceId { get; set; }
        public string LinkGroupChat { get; set; }
    }

    public class ChapterViewModelReturn
    {
        public ChapterViewModelReturn()
        {
            
        }
        public string ProvinceName { get; set; }
        public string RegionName { get; set; }
        public string ChapterName { get; set; }
        public int ChapterId { get; set; }
    }
}
