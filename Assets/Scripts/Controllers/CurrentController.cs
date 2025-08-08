using UnityEngine;

[RequireComponent(typeof(CurrentView))]
public class CurrentController : MonoBehaviour
{
    [SerializeField] private CurrentView _currentView;
    public void SendBulletToCurrent()
    {
        BulletData bulletToSpawn = CenterManager.Instance.TakeRandomBulletFromCenter();
        CurrentSpace[] chosenCurrentColumn = _currentView.GetCurrentColumnByColor((int)bulletToSpawn.Color);

        int spacesToMoveDown = bulletToSpawn.Number;
        int spacesMoved = 0;
        int spaceToSpawnInto;

        for (spaceToSpawnInto = 0; spaceToSpawnInto < chosenCurrentColumn.Length; spaceToSpawnInto++)
        {
            if (chosenCurrentColumn[spaceToSpawnInto].BulletProperties == null) spacesMoved++;
            if (spacesMoved >= spacesToMoveDown)
            {
                Debug.Log("Space found in time!");
                break;
            }
        }

        if(spacesMoved >= spacesToMoveDown)
        {
            _currentView.SpawnNewCurrentBulletObject(spaceToSpawnInto, (int)bulletToSpawn.Color, bulletToSpawn);
        }
        else
        {
            Debug.Log("No space remaining! Life lost!");
            GameManager.Instance.Player1.ModifyCurrentHP(-1);
        }
    }
}
