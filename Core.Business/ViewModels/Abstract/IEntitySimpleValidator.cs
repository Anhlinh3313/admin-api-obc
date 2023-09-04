using System;
namespace Core.Business.ViewModels.Abstract
{
    public interface IEntitySimpleValidator
    {
        bool IsIdExist(int id);
        bool IsCodeExist(string code);
        bool IsNameExist(string name);
    }
}
