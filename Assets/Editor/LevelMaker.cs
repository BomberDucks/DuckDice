using UnityEngine;
using UnityEditor; // Indispensable pour l'éditeur
using System.Text; // Pour le StringBuilder

public class LevelEditorWindow : EditorWindow
{
    
    public LevelData levelData;

    // Outils et etat
    private char selectedTool = '#';
    private char[,] mapGrid;
    
    // Taille de basse
    private int width = 10;
    private int height = 10;
    
    // Position de l'ascenseur (scroll)
    private Vector2 scrollPos;

    // Ajoute le menu dans Unity en haut
    [MenuItem("Sokoban/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Éditeur de Niveau Sokoban", EditorStyles.boldLabel);
        
        // selectionne le fichier level data
        levelData = (LevelData)EditorGUILayout.ObjectField("Fichier LevelData", levelData, typeof(LevelData), false);

        if (levelData == null)
        {
            EditorGUILayout.HelpBox("Veuillez glisser un fichier LevelData ci-dessus pour commencer.", MessageType.Info);
            return;
        }

        GUILayout.Space(5);

        // pour load save les fichier leveldata
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load", GUILayout.Height(30))) LoadLevel();
        if (GUILayout.Button("Save", GUILayout.Height(30))) SaveLevel();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // resizing de mes maps
        GUILayout.Label("Dimensions de la Grille", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        width = EditorGUILayout.IntField("Largeur (X)", width);
        height = EditorGUILayout.IntField("Hauteur (Y)", height);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        // resize auto
        if (GUILayout.Button("Apply size"))
        {
            ResizeGrid(keepData: true);
        }
        
        // reset
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("Tout Effacer (Reset)"))
        {
            if(EditorUtility.DisplayDialog("Attention", "Voulez-vous vraiment tout effacer ?", "Oui", "Annuler"))
            {
                ResizeGrid(keepData: false);
            }
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // outils de modif
        GUILayout.Label("Palette d'Outils (Cliquez pour sélectionner)", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        DrawToolButton('#', "Mur", Color.gray);
        DrawToolButton('.', "Sol", Color.white);
        DrawToolButton('P', "Joueur", Color.green);
        DrawToolButton('D', "Dé", Color.cyan);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        DrawToolButton('1', "1", Color.yellow);
        DrawToolButton('2', "2", Color.yellow);
        DrawToolButton('3', "3", Color.yellow);
        DrawToolButton('4', "4", Color.yellow);
        DrawToolButton('5', "5", Color.yellow);
        DrawToolButton('6', "6", Color.yellow);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        // grille centree
        if (mapGrid != null)
        {
            // scroll
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            // centrage
            GUILayout.BeginHorizontal(); 
            GUILayout.FlexibleSpace(); // ressort Gauche
            
            GUILayout.BeginVertical(); // bloc Vertical pour la grille

            for (int y = 0; y < mapGrid.GetLength(1); y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < mapGrid.GetLength(0); x++)
                {
                    char cell = mapGrid[x, y];
                    
                    // donne une couleur aux outils en fonction de la selection
                    GUI.backgroundColor = GetColorForChar(cell);

                    // dessin du bouton 
                    if (GUILayout.Button(cell.ToString(), GUILayout.Width(35), GUILayout.Height(35)))
                    {
                        // verifie le click
                        if (Event.current.button == 0)
                        {
                            mapGrid[x, y] = selectedTool;
                            //actualise ma fenetre
                            GUI.changed = true; 
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical(); 

            GUILayout.FlexibleSpace(); // Ressort Droite
            GUILayout.EndHorizontal();
           
            
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndScrollView();
        }
    }
    

    void ResizeGrid(bool keepData)
    {
        if (width < 1) width = 1;
        if (height < 1) height = 1;

        char[,] newGrid = new char[width, height];

        // remplir de vide
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                newGrid[x, y] = '.';

        // recupere ancienne data
        if (keepData && mapGrid != null)
        {
            int oldW = mapGrid.GetLength(0);
            int oldH = mapGrid.GetLength(1);

            for (int x = 0; x < Mathf.Min(width, oldW); x++)
            {
                for (int y = 0; y < Mathf.Min(height, oldH); y++)
                {
                    newGrid[x, y] = mapGrid[x, y];
                }
            }
        }
        mapGrid = newGrid;
    }

    void LoadLevel()
    {
        if (levelData == null || string.IsNullOrEmpty(levelData.LevelString)) return;

        string[] lines = levelData.LevelString.Split('\n');
        
        height = lines.Length;
        width = 0;
        foreach(var line in lines) if(line.Trim().Length > width) width = line.Trim().Length;

        // reset
        ResizeGrid(false);

        for (int y = 0; y < height; y++)
        {
            string line = lines[y].Trim();
            for (int x = 0; x < width; x++)
            {
                if (x < line.Length) mapGrid[x, y] = line[x];
            }
        }
        Debug.Log("Niveau chargé !");
    }

    void SaveLevel()
    {
        if (levelData == null || mapGrid == null) return;

        StringBuilder sb = new StringBuilder();
        int w = mapGrid.GetLength(0);
        int h = mapGrid.GetLength(1);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                sb.Append(mapGrid[x, y]);
            }
            if (y < h - 1) sb.Append("\n");
        }

        levelData.LevelString = sb.ToString();
        levelData.Width = w;
        levelData.Height = h;

        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();
        Debug.Log("Niveau sauvegardé !");
    }
    

    void DrawToolButton(char symbol, string name, Color color)
    {
        // Si l'outil est sélectionné, on le met en Rouge, sinon sa couleur normale
        GUI.backgroundColor = (selectedTool == symbol) ? Color.red : color;
        
        if (GUILayout.Button(name, GUILayout.Width(50), GUILayout.Height(30)))
        {
            selectedTool = symbol;
        }
        GUI.backgroundColor = Color.white;
    }

    Color GetColorForChar(char c)
    //gestion des couleurs
    {
        switch (c)
        {
            case '#': return Color.gray;
            case '.': return Color.white;
            case 'P': return Color.green;
            case 'D': return Color.cyan;
            case '1': case '2': case '3': case '4': case '5': case '6': return Color.yellow;
            default: return Color.white;
        }
    }
}