using SystemBase;
using Systems.Tourist;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[GameSystem]
public class UISystem : GameSystem<UIComponent>
{

    public override void Register(UIComponent component)
    {
        MessageBroker.Default.Receive<ShowDeadPersonAction>()
            .Subscribe(msg =>
            {
                component.Message.gameObject.SetActive(true);

                var text = component.Message.Text;
                var face = component.Message.Image;

                text.text = msg.TouristName + " died.";
                var faces = component.GetComponentInParent<TouristConfigComponent>().topParts;
                face.sprite = faces[msg.TouristFaceIndex];
            })
            .AddTo(component);
    }
}
