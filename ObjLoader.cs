namespace BPX;

using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;

public class ObjModel
{
    public required float[] Vertices { get; set; }
    public required uint[] Indices { get; set; }
}

public static class ObjLoader
{
    public static ObjModel Load(string path)
    {
        var positions = new List<Vector3>();
        var indices = new List<uint>();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (tokens.Length == 0) continue;

            string type = tokens[0];

            if (type == "v")
            {
                float x = float.Parse(tokens[1], System.Globalization.CultureInfo.InvariantCulture);
                float y = float.Parse(tokens[2], System.Globalization.CultureInfo.InvariantCulture);
                float z = float.Parse(tokens[3], System.Globalization.CultureInfo.InvariantCulture);
                
                positions.Add(new Vector3(x, y, z));
            }
            else if (type == "f")
            {
                
                int[] faceIndices = new int[tokens.Length - 1];
                
                for (int i = 1; i < tokens.Length; i++)
                {
                    string token = tokens[i];
                    string vertexIndexStr = token.Split('/')[0];
                    
                    int index = int.Parse(vertexIndexStr, System.Globalization.CultureInfo.InvariantCulture);
                    
                    if (index > 0)
                        faceIndices[i - 1] = index - 1;
                    else
                        faceIndices[i - 1] = positions.Count + index;
                }

                for (int i = 1; i < faceIndices.Length - 1; i++)
                {
                    indices.Add((uint)faceIndices[0]);
                    indices.Add((uint)faceIndices[i]);
                    indices.Add((uint)faceIndices[i + 1]);
                }
            }
        }

        

        float[] verticesArray = new float[positions.Count * 3];
        for (int i = 0; i < positions.Count; i++)
        {
            verticesArray[i * 3]     = positions[i].X;
            verticesArray[i * 3 + 1] = positions[i].Y;
            verticesArray[i * 3 + 2] = positions[i].Z;
        }

        return new ObjModel
        {
            Vertices = verticesArray,
            Indices = indices.ToArray()
        };
    }
}