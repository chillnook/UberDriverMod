using GTA.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Screen = GTA.UI.Screen;

namespace UberDriverMod
{
    static internal class UberProfile
    {
        public static bool draw = false;

        public static TextElement profileTitle;
        public static CustomSprite profileImg;
        public static ContainerElement profileBackground;
        public static TextElement profileName;

        public static TextElement profileTripsTitle;
        public static TextElement profileTripsStat;

        public static TextElement profileRatingTitle;
        public static TextElement profileRatingStat;
        public static CustomSprite profileRatingImg;

        public static TextElement profileJoinDateTitle;
        public static TextElement profileJoinDateStat;

        public static void CreateUberProfile()
        {
            

            var titlePos = new PointF(Screen.Width / 2, Screen.Height / 2 - 290);
            profileTitle = new TextElement("Driver Profile", titlePos, 0.8f, Color.FromArgb(255, 255, 255, 255), GTA.UI.Font.Monospace, Alignment.Center, true, true);
            profileTitle.Enabled = true;

            var backgroundSize = new SizeF(300, 400);
            var backgroundPos = new PointF(Screen.Width / 2 - backgroundSize.Width / 2, Screen.Height / 2 - backgroundSize.Height / 2 - 100);
            profileBackground = new ContainerElement(backgroundPos, backgroundSize, Color.FromArgb(165, 0, 0, 0));
            profileBackground.Enabled = true;

            var imageSize = new SizeF(200, 200);
            var imagePos = new PointF(Screen.Width / 2 - imageSize.Width / 2, Screen.Height / 2 - imageSize.Height / 2 - 140);
            profileImg = new CustomSprite("scripts\\UberDriver\\Textures\\profile.jpg", imageSize, imagePos, Color.White);
            profileImg.Enabled = true;
            
            var namePos = new PointF(Screen.Width / 2, Screen.Height / 2 - 40);
            profileName = new TextElement(MainClass.config.GetValue("Profile", "Name", "John Doe"), namePos, 0.85f, Color.White, GTA.UI.Font.Monospace, Alignment.Center, true, true);
            profileName.Enabled = true;

            var tripsTitlePos = new PointF(Screen.Width / 2 - 90, Screen.Height / 2 + 53);
            profileTripsTitle = new TextElement("Trips", tripsTitlePos, 0.4f, Color.White, GTA.UI.Font.Monospace, Alignment.Center, true, true);
            profileTripsTitle.Enabled = true;

            var tripsStatsPos = new PointF(Screen.Width / 2 - 90, Screen.Height / 2 + 23);
            profileTripsStat = new TextElement(MainClass.config.GetValue("Profile", "Trips", "0"), tripsStatsPos, 0.8f, Color.White, GTA.UI.Font.Monospace, Alignment.Center, true, true);
            profileTripsStat.Enabled = true;

            var ratingTitlePos = new PointF(Screen.Width / 2, Screen.Height / 2 + 53);
            profileRatingTitle = new TextElement("Rating", ratingTitlePos, 0.4f, Color.White, GTA.UI.Font.Monospace, Alignment.Center, true, true);
            profileRatingTitle.Enabled = true;

            var ratingStatsPos = new PointF(Screen.Width / 2 - 9, Screen.Height / 2 + 23);
            profileRatingStat = new TextElement(MainClass.config.GetValue("Profile", "Rating", "5"), ratingStatsPos, 0.8f, Color.White, GTA.UI.Font.Monospace, Alignment.Center, true, true);
            profileRatingStat.Enabled = true;

            var profileRatingImgSize = new SizeF(13, 13);
            var profileRatingImgPos = new PointF(Screen.Width / 2 - profileRatingImgSize.Width / 2 + 6, Screen.Height / 2  - profileRatingImgSize.Height / 2 + 42);
            profileRatingImg = new CustomSprite("scripts\\UberDriver\\Textures\\star.png", profileRatingImgSize, profileRatingImgPos, Color.White);
            profileRatingImg.Enabled = true;

            var joinDateTitlePos = new PointF(Screen.Width / 2 + 90, Screen.Height / 2 + 53);
            profileJoinDateTitle = new TextElement("Days", joinDateTitlePos, 0.4f, Color.White, GTA.UI.Font.Monospace, Alignment.Center, true, true);
            profileJoinDateTitle.Enabled = true;

            var joinDateStatsPos = new PointF(Screen.Width / 2 + 90, Screen.Height / 2 + 23);
            profileJoinDateStat = new TextElement(MainClass.config.GetValue("Profile", "StartDate", "0"), joinDateStatsPos, 0.8f, Color.White, GTA.UI.Font.Monospace, Alignment.Center, true, true);
            profileJoinDateStat.Enabled = true;
        }

        public static void DrawUberProfile()
        {
            try
            {
                if (draw == false) return;
                profileBackground.Draw();
                profileTitle.Draw();
                profileImg.Draw();
                profileName.Draw();
                profileTripsTitle.Draw();
                profileTripsStat.Draw();
                profileRatingTitle.Draw();
                profileRatingStat.Draw();
                profileRatingImg.Draw();
                profileJoinDateTitle.Draw();
                profileJoinDateStat.Draw();

                UpdateUberProfile();
            } catch { }
            
        }

        private static void UpdateUberProfile()
        {
            profileName.Caption = MainClass.config.GetValue("Profile", "Name", "John Doe");
        }
    }
}
