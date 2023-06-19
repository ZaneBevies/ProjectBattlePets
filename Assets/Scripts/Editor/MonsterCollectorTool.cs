using UnityEditor;
using UnityEngine;

public class MonsterCollectorTool : EditorWindow
{
    public string customName;
    public int level = 1;
    public MonsterSO baseMonsterInfo;

    [MenuItem("Tools/Monster Collector Tool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(MonsterCollectorTool));
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Monster To Collection", EditorStyles.boldLabel);

        customName = EditorGUILayout.TextField("Custom Monster Name", customName);
        level = EditorGUILayout.IntSlider("Monster Level", level, 1, 999);
        baseMonsterInfo = EditorGUILayout.ObjectField("Monster Scriptable Object", baseMonsterInfo, typeof(MonsterSO), false) as MonsterSO;

        if (GUILayout.Button("Add Monster To Collection"))
        {
            AddMonsterCollection();
        }

        

        if (GUILayout.Button("Add Monster To Party"))
        {
            AddMonsterParty();
        }

        if (GUILayout.Button("Clear Collection"))
        {
            ClearMonsters();
        }

        if (GUILayout.Button("Clear Party"))
        {
            ClearMonstersFromParty();
        }


        GUILayout.Label("Editor Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Switch To Overworld"))
        {
            MoveEnvironment("Overworld");
        }

        if (GUILayout.Button("Switch To Battle"))
        {
            MoveEnvironment("Battle");
        }

        if (GUILayout.Button("Switch To Collection"))
        {
            MoveEnvironment("Collection");
        }
    }


    private void MoveEnvironment(string name)
    {
        GameManager man = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        if (name == "Overworld")
        {
            man.overworldGameobject.SetActive(true);
            man.overworldUI.gameObject.SetActive(true);
            man.battleGameobject.SetActive(false);
            man.battleUI.gameObject.SetActive(false);
            man.collectionManager.mainInterface.SetActive(false);
        }
        else if (name == "Battle")
        {
            man.overworldGameobject.SetActive(false);
            man.overworldUI.gameObject.SetActive(false);
            man.battleGameobject.SetActive(true);
            man.battleUI.gameObject.SetActive(true);
            man.collectionManager.mainInterface.SetActive(false);
        }
        else if (name == "Collection")
        {
            man.overworldGameobject.SetActive(true);
            man.overworldUI.gameObject.SetActive(false);
            man.battleGameobject.SetActive(false);
            man.battleUI.gameObject.SetActive(false);
            man.collectionManager.mainInterface.SetActive(true);
        }
    }
    private void AddMonsterCollection()
    {
        CollectionManager collectionManager = GameObject.FindGameObjectWithTag("CollectionManager").GetComponent<CollectionManager>();

        if (collectionManager == null) return;

        Monster monster = new Monster(customName, level, baseMonsterInfo);



        collectionManager.SpawnMonsterInCollection(monster);


    }

    private void AddMonsterParty()
    {
        CollectionManager collectionManager = GameObject.FindGameObjectWithTag("CollectionManager").GetComponent<CollectionManager>();
        if (collectionManager == null) return;

        if (collectionManager.CheckFreePartySlot() == 0) return;
        

        Monster monster = new Monster(customName, level, baseMonsterInfo);

        collectionManager.SpawnMonsterInParty(monster, collectionManager.CheckFreePartySlot());


    }

    private void ClearMonsters()
    {
        CollectionManager collectionManager = GameObject.FindGameObjectWithTag("CollectionManager").GetComponent<CollectionManager>();

        if (collectionManager == null) return;

        collectionManager.ClearAllMonstersFromCollection();
    }

    private void ClearMonstersFromParty()
    {
        CollectionManager collectionManager = GameObject.FindGameObjectWithTag("CollectionManager").GetComponent<CollectionManager>();

        if (collectionManager == null) return;

        collectionManager.ClearAllMonstersFromParty();
    }
}
