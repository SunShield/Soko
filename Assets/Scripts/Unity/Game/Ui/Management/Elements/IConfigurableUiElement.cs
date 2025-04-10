namespace Soko.Unity.Game.Ui.Management.Elements
{
    public interface IConfigurableUiElement<in TConfigureData>
    {
        void Configure(TConfigureData data);
    }
}