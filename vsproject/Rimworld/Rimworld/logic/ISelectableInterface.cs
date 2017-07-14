
namespace Rimworld.logic
{
    public interface ISelectableInterface
    {
        string GetName();
        string GetDescription();
        string GetHitPointString();	// For indestructible things (if any?) this is allowed to return blank (or null maybe??)
    }
}
