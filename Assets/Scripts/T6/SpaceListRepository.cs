using System;
using System.Collections.Generic;
using UnityEngine;

public class T6SpaceListRepository : MonoBehaviour
{
    public static T6SpaceListRepository Instance { get; private set; }

    [SerializeField] 
    private T6SpaceListData listData = new T6SpaceListData();

    private string localFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        localFilePath = System.IO.Path.Combine(
            Application.persistentDataPath, 
            "space_list.json"
            );  

        DontDestroyOnLoad(gameObject);
    }

    // 목록 초기화
    public void SetList(List<T6SpaceSummary> spaces)
    {
        listData.spaces = spaces;
    }

    // 단일 공간 추가
    public void Add(T6SpaceSummary summary)
    {
        listData.spaces.Add(summary);
        SaveLocal(localFilePath);
    }

    // 단일 공간 삭제
    public void Remove(long id)
    {
        listData.spaces.RemoveAll(x => x.id == id);
        SaveLocal(localFilePath);
    }   

    // 단일 항목 이름 수정
    public void UpdateName(long id, string newName)
    {
        var found = listData.spaces.Find(x => x.id == id);
        if (found != null)
        {
            found.name = newName;
            SaveLocal(localFilePath);
        }
    }

    // 전체 리스트 반환
    public List<T6SpaceSummary> GetAll()
    {
        return listData.spaces;
    }

    // 로컬 저장/로드(선택 사항, 나중 단계에서 구현)
    public void SaveLocal(string path)
    {
        var json = JsonUtility.ToJson(listData, true);
        System.IO.File.WriteAllText(path, json);
    }
    public void SaveLocal()
    {
        SaveLocal(localFilePath);
    }

    public void LoadLocal(string path)
    {
        Debug.Log(localFilePath);
        if (System.IO.File.Exists(path))
        {
            var json = System.IO.File.ReadAllText(path);
            listData = JsonUtility.FromJson<T6SpaceListData>(json);
        }
    }
    public void LoadLocal()
    {
        LoadLocal(localFilePath);
    }
}
