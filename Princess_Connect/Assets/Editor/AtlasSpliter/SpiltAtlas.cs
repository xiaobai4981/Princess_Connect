using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace XANEditor
{
    public static class ImageSlicerTool
    {
        [MenuItem("Assets/ImageSlicer/Process to Sprites")]
        static void ProcessToSprite()
        {
            Texture2D image = Selection.activeObject as Texture2D;//��ȡѡ��Ķ���
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image));//��ȡ·������
            string fileExtension = Path.GetExtension(AssetDatabase.GetAssetPath(image));//��ȡ·�����ƺ�׺������չ����
            string path = rootPath + "/" + image.name + fileExtension;//ͼƬ·������


            TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;//��ȡͼƬ���
            if (texImp.spritesheet.Length <= 0)
            {
                Debug.LogError("û�о���ͼ�����޷��и���ͼƬתΪ sprite - Multiple �������ʵ��ָ��¾���ͼ");
                return;
            }

            AssetDatabase.CreateFolder(rootPath, image.name);//�����ļ���
            foreach (SpriteMetaData metaData in texImp.spritesheet)//����Сͼ��
            {
                Texture2D myimage = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);

                //abc_0:(x:2.00, y:400.00, width:103.00, height:112.00)
                for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++)//Y������
                {
                    for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                        myimage.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, image.GetPixel(x, y));
                }


                //ת������EncodeToPNG���ݸ�ʽ
                if (myimage.format != TextureFormat.ARGB32 && myimage.format != TextureFormat.RGB24)
                {
                    Texture2D newTexture = new Texture2D(myimage.width, myimage.height);
                    newTexture.SetPixels(myimage.GetPixels(0), 0);
                    myimage = newTexture;
                }
                var pngData = myimage.EncodeToPNG();


                File.WriteAllBytes(rootPath + "/" + image.name + "/" + metaData.name + fileExtension, pngData);
                // ˢ����Դ���ڽ���
                AssetDatabase.Refresh();
            }
        }
    }
}