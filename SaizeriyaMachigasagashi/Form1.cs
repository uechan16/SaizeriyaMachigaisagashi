using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.Extensions; 
using System.Diagnostics;
using OpenCvSharp.Util;
using System.Drawing;
using Size = OpenCvSharp.Size;
using Point = OpenCvSharp.Point;

namespace SaizeriyaMachigasagashi
{
    public partial class Form1 : Form
    {
        string sRightPictureFile = "";
        string sLeftPictureFile = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "default.jpg";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "画像ファイル(*.jpg)|*.jpg|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開く画像ファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                sLeftPictureFile = ofd.FileName;
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.ImageLocation = sLeftPictureFile;

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(sLeftPictureFile == "")
            {
                return;
            }
            if (sRightPictureFile == "")
            {
                return;
            }
            Task task = Task.Run(() => {
                FindContours(sLeftPictureFile, sRightPictureFile);
            });

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "default.jpg";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "画像ファイル(*.jpg)|*.jpg|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開く画像ファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                sRightPictureFile = ofd.FileName;
            }
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.ImageLocation = sRightPictureFile;
        }
        public void FindContours(string sLeftPictureFile, string sRightPictureFile)
        {
            Mat tokuLeft = new Mat();
            Mat tokuRight = new Mat();
            Mat output = new Mat();

            AKAZE akaze = AKAZE.Create();
            KeyPoint[] keyPointsLeft;
            KeyPoint[] keyPointsRight;

            Mat descriptorLeft = new Mat();  
            Mat descriptorRight = new Mat();

            DescriptorMatcher matcher; //マッチング方法
            DMatch[] matches; //特徴量ベクトル同士のマッチング結果を格納する配列

            //画像をグレースケールとして読み込み、平滑化する  
            Mat Lsrc = new Mat(sLeftPictureFile, ImreadModes.Color);
            //Cv2.Resize(Lsrc, Lsrc, new Size(1980, 1080));

            //Lsrc.Blur(new Size(3, 3));
            //Cv2.AdaptiveThreshold(Lsrc, Lsrc, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 9, 12);

            //画像をグレースケールとして読み込み、平滑化する  
            Mat Rsrc = new Mat(sRightPictureFile, ImreadModes.Color);
            //Cv2.Resize(Rsrc, Rsrc, new Size(1980, 1080));

            //Rsrc.Blur(new Size(3, 3));
            //Cv2.AdaptiveThreshold(Rsrc, Rsrc, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 9, 12);

           
            //特徴量の検出と特徴量ベクトルの計算
            akaze.DetectAndCompute(Lsrc, null, out keyPointsLeft, descriptorLeft);
            akaze.DetectAndCompute(Rsrc, null, out keyPointsRight, descriptorRight);
            

            //画像1の特徴点をoutput1に出力
            Cv2.DrawKeypoints(Lsrc, keyPointsLeft, tokuLeft);
            Image imageLeftToku = BitmapConverter.ToBitmap(tokuLeft);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = imageLeftToku;

            //画像2の特徴点をoutput1に出力
            Cv2.DrawKeypoints(Rsrc, keyPointsRight, tokuRight);
            Image imageRightToku = BitmapConverter.ToBitmap(tokuRight);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = imageRightToku;

            //総当たりマッチング
            matcher = DescriptorMatcher.Create("BruteForce");
            matches = matcher.Match(descriptorLeft, descriptorRight);

            Cv2.DrawMatches(Lsrc, keyPointsLeft, Rsrc, keyPointsRight, matches, output);
            output.SaveImage("output.jpg");

            int size = matches.Count();
            var getPtsSrc = new Vec2f[size];
            var getPtsTarget = new Vec2f[size];

            int count = 0;
            foreach (var item in matches)
            {
                var ptSrc = keyPointsLeft[item.QueryIdx].Pt;
                var ptTarget = keyPointsRight[item.TrainIdx].Pt;
                getPtsSrc[count][0] = ptSrc.X;
                getPtsSrc[count][1] = ptSrc.Y;
                getPtsTarget[count][0] = ptTarget.X;
                getPtsTarget[count][1] = ptTarget.Y;
                count++;
            }

            // SrcをTargetにあわせこむ変換行列homを取得する。ロバスト推定法はRANZAC。
            var hom = Cv2.FindHomography(
                InputArray.Create(getPtsSrc),
                InputArray.Create(getPtsTarget),
                HomographyMethods.Ransac);

            // 行列homを用いてSrcに射影変換を適用する。
            Mat WarpedSrcMat = new Mat();
            Cv2.WarpPerspective(
                Lsrc, WarpedSrcMat, hom,
                new OpenCvSharp.Size(Rsrc.Width, Rsrc.Height));

            WarpedSrcMat.SaveImage("Warap.jpg");

            //画像1の特徴点をoutput1に出力
            Image imageLeftSyaei = BitmapConverter.ToBitmap(WarpedSrcMat);
            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.Image = imageLeftSyaei;

            //画像2の特徴点をoutput1に出力
            Image imageRightSyaei = BitmapConverter.ToBitmap(Rsrc);
            pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox6.Image = imageRightSyaei;

            /*
            Mat diff = new Mat();
            Cv2.Absdiff(WarpedSrcMat, Rsrc,diff);
            diff.SaveImage("diff.jpg");

            Mat diffBitWise = new Mat();
            Cv2.BitwiseOr(WarpedSrcMat, Rsrc, diffBitWise);
            diff.SaveImage("diffBitWise.jpg");

            */

            Mat LmatFloat = new Mat();
            WarpedSrcMat.ConvertTo(LmatFloat, MatType.CV_16SC3);
            Mat[] LmatPlanes = LmatFloat.Split();

            Mat RmatFloat = new Mat();
            Rsrc.ConvertTo(RmatFloat, MatType.CV_16SC3);
            Mat[] RmatPlanes = RmatFloat.Split();

            Mat diff0 = new Mat();
            Mat diff1 = new Mat();
            Mat diff2 = new Mat();

            Cv2.Absdiff(LmatPlanes[0], RmatPlanes[0], diff0);
            Cv2.Absdiff(LmatPlanes[1], RmatPlanes[1], diff1);
            Cv2.Absdiff(LmatPlanes[2], RmatPlanes[2], diff2);

            Cv2.MedianBlur(diff0, diff0, 5);
            Cv2.MedianBlur(diff1, diff1, 5);
            Cv2.MedianBlur(diff2, diff2, 5);

            Mat wiseMat = new Mat();
            Cv2.BitwiseOr(diff0, diff1, wiseMat);
            Cv2.BitwiseOr(wiseMat, diff2, wiseMat);

            Mat openingMat = new Mat();
            Cv2.MorphologyEx(wiseMat, openingMat, MorphTypes.Open,new Mat());

            Mat dilationMat = new Mat();
            Cv2.Dilate(openingMat, dilationMat, new Mat());
            dilationMat.SaveImage("dilationMat.jpg");

            Mat LaddMat = new Mat();
            Mat RaddMat = new Mat();
            Console.WriteLine(dilationMat.GetType());
            Console.WriteLine(Rsrc.GetType());

            Cv2.CvtColor(dilationMat,dilationMat, ColorConversionCodes.BGR2RGB);

            Cv2.Add(WarpedSrcMat, dilationMat, LaddMat);
            Cv2.Add(Rsrc, dilationMat, LaddMat);

            Cv2.ImShow("test", RaddMat);

            Image LaddImage = BitmapConverter.ToBitmap(LaddMat);
            pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox7.Image = LaddImage;

            Image RaddImage = BitmapConverter.ToBitmap(RaddMat);
            pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox8.Image = RaddImage;


        }

    }
}
