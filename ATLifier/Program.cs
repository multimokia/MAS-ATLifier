using System;
using System.Collections.Generic;

using folderStuffs;

namespace ATLifier
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set up our file io
            FolderReview file = new FolderReview();

            //Data from sprite chart
            List<string> sprite_chart_old = file.read("sprite-chart.rpy","Old");

            //The new sprite chart
            List<string> sprite_chart_new = new List<string>();

            //The data we want:
            List<List<string>> sprite_definitions_old = new List<List<string>>();

            //The new sprite defs
            List<List<string>> sprite_definitions_new = new List<List<string>>();

            int line_count = 0;

            //Now we go thru the data
            //First, we get rid of the 4k lines of code
            foreach (string line in sprite_chart_old)
            {
                if (line.Contains("image monika") && line.Contains("DynamicDisplayable"))
                    break;

                sprite_chart_new.Add(line);
                line_count++;
            }
            sprite_chart_old.RemoveRange(0,line_count);

            //Now we iter and get all the sprite defs as string lists
            List<string> sprite = new List<string>();

            foreach (string line in sprite_chart_old)
            {
                if (line.Contains("image monika") && line.Contains("DynamicDisplayable"))
                {
                    //Don't add this if it's not needed
                    if (sprite.Count != 0)
                        sprite_definitions_old.Add(sprite);

                    //Reset this
                    sprite = new List<string>();
                }

                //This is all excess stuff that we want, but not for amending sprites
                if (line.Contains("### [IMG032]"))
                    break;

                sprite.Add(line);
            }

            //Add the last sprite
            sprite_definitions_old.Add(sprite);

            //Get rid of all the sprites so all that's left is the extra defs/ATLs
            foreach (List<string> strL in sprite_definitions_old)
            {
                foreach (string s in strL)
                {
                    sprite_chart_old.Remove(s);
                }
            }

            //Now we create and add our new sprite objects
            foreach (List<string> strL in sprite_definitions_old)
            {
                sprite_chart_new.AddRange(CreateNewSprite(strL));
            }



            sprite_chart_new.AddRange(sprite_chart_old);

            file.create("sprite-chart.rpy","New",sprite_chart_new,overwrite: true);
        }

        public static List<string> CreateNewSprite(List<string> old_sprite_def)
        {
            //So we don't do extra work
            foreach (string s in old_sprite_def)
            {
                if (s.Contains("closedsad") || s.Contains("closedhappy") || s.Contains("wink"))
                    return old_sprite_def;
            }

            List<string> new_sprite_def = new List<string>();

            new_sprite_def.Add($"image monika {old_sprite_def[0].Substring(13, old_sprite_def[0].IndexOf("=") - 14)}:");
            new_sprite_def.Add("    block:");

            new_sprite_def.Add("        DynamicDisplayable(");
            foreach (string s in old_sprite_def)
            {
                if (!s.Contains("image monika"))
                    if (s.Equals(""))
                        new_sprite_def.Add("");
                    else
                        new_sprite_def.Add("        " + s);
            }

            new_sprite_def.Add("        block:");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                7");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                8");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                9");
            new_sprite_def.Add("");

            new_sprite_def.Add("        DynamicDisplayable(");
            foreach (string s in old_sprite_def)
            {
                if (!s.Contains("image monika"))
                    if (s.Contains("eyes"))
                        new_sprite_def.Add("            eyes=\"closedsad\",");
                    else
                        if (s.Equals(""))
                            new_sprite_def.Add("");
                        else
                            new_sprite_def.Add("        " + s);
            }

            new_sprite_def.Add("        0.05");
            new_sprite_def.Add("        repeat");
            new_sprite_def.Add("");
            return new_sprite_def;
        }
    }
}
