using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager {
    private static Dictionary<string, Sprite> nameSpriteDic = new Dictionary<string, Sprite>();
    private static Dictionary<int, Sprite> numberSpriteDic = new Dictionary<int, Sprite>();
    private static Dictionary<int, Sprite> diceSpriteDic = new Dictionary<int, Sprite>();
    /// <summary>
    /// 用来获取头像
    /// </summary>
    /// <param name="iconName"></param>
    /// <returns></returns>
	public static Sprite GetSprite(string iconName)
    {
        if (nameSpriteDic.ContainsKey(iconName))
        {
            return nameSpriteDic[iconName];
        }
        else
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("headIcon");
            string[] nameArr = iconName.Split('_');
            Sprite temp = sprites[(int.Parse(nameArr[1])-1)];
            nameSpriteDic.Add(iconName, temp);
            return temp;
        }

    }
    /// <summary>
    /// 获得数字的图片
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static Sprite GetNumSprite(int num)
    {
        if (numberSpriteDic.ContainsKey(num))
        {
            return numberSpriteDic[num];
        }
        else
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("number");
            for (int i = 0; i < sprites.Length; i++)
            {
                //如果序号和名字相同
                if(num == int.Parse(sprites[i].name))
                {
                    Sprite temp = sprites[i];
                    numberSpriteDic.Add(num, temp);
                    return temp;
                }
            }
            return null;
        }
    }
    /// <summary>
    /// 获得点数对应的骰子的图片
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static Sprite GetDiceSprite(int num)
    {
        if (diceSpriteDic.ContainsKey(num))
        {
            return diceSpriteDic[num];
        }
        else
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("dice");
            Sprite temp = sprites[num-1];
            diceSpriteDic.Add(num, temp);
            return temp;
        }
    }
}
