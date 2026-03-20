using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializeReference), true)]
public class SerializeReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 1. 라벨 그리기
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        // 2. 클래스 선택 버튼 (드롭다운)
        Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

        string typeName = property.managedReferenceFullTypename;
        string displayTypeName = string.IsNullOrEmpty(typeName) ? "None (Null)" : typeName.Split(' ').Last().Split('.').Last();

        if (GUI.Button(buttonRect, displayTypeName, EditorStyles.layerMaskField))
        {
            ShowTypeMenu(property);
        }

        // 3. 내부 필드들 그리기 (리플렉션으로 확장된 필드들)
        EditorGUI.PropertyField(position, property, label, true);

        EditorGUI.EndProperty();
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();
        Type targetType = GetPropertyType(property);

        // 현재 프로젝트에서 해당 인터페이스를 구현하는 모든 클래스 찾기
        var types = TypeCache.GetTypesDerivedFrom(targetType)
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsSerializable);

        menu.AddItem(new GUIContent("None"), false, () => {
            property.managedReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
        });

        foreach (var type in types)
        {
            menu.AddItem(new GUIContent(type.Name), false, () => {
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            });
        }
        menu.ShowAsContext();
    }

    private Type GetPropertyType(SerializedProperty property)
    {
        // "managedReferenceFieldTypename"에서 타입 정보 추출
        string[] parts = property.managedReferenceFieldTypename.Split(' ');
        return Assembly.Load(parts[0]).GetType(parts[1]);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}