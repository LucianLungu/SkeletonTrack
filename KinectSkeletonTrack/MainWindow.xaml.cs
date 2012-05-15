using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor __kinect;

        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];

        private struct JPoint
        {
            public float X;
            public float Y;

        }

        private JPoint handLeft;

        const int CanvasWidth = 640;
        const int CanvasHeight = 480;
        public MainWindow()
        {
            InitializeComponent();
            SetupKinect();
        }
                    //setup kinect
            void SetupKinect( )
        {
            //get first kinect sensor
            if ( KinectSensor.KinectSensors.Count > 0 )
            {
                __kinect = KinectSensor.KinectSensors[ 0 ];

                if ( __kinect.Status == KinectStatus.Connected )
                {
                    //start the video stream only if the program in run from the
                    //Calibration Menu

                    __kinect.ColorStream.Enable( ColorImageFormat.RgbResolution640x480Fps30 );

                    __kinect.DepthStream.Enable();
                    __kinect.SkeletonStream.Enable();
                    // init the Depth Stream, with Near Mode Enabled
                    //KinectSensor.DepthStream.Enable( DepthImageFormat.Resolution640x480Fps30 );
                    //KinectSensor.DepthStream.Range = DepthRange.Near;

                    __kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>( _sensor_AllFramesReady );

                    __kinect.Start( );
                }
            }
        }


        void _sensor_AllFramesReady( object sender, AllFramesReadyEventArgs e )
        {
            Skeleton __skeleton = GetFirstSkeleton(e);

            if (__skeleton == null)
                return;
            GetJointLocations(__skeleton);
            DrawSkeleton();

   


        }

        Skeleton GetFirstSkeleton (AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame()) 
            { 
                if (skeletonFrameData == null)                    
                    return null; 
                skeletonFrameData.CopySkeletonDataTo(allSkeletons); 
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked 
                                  select s).FirstOrDefault();
                return first; 
            }
        }
        private void GetJointLocations(Skeleton thisSkeleton)
        {
            handLeft = JointLocation(thisSkeleton.Joints[JointType.HandLeft]);
        }
            private JPoint JointLocation(Joint joint) 
            { JPoint tmp; var scaledJoint = joint.ScaleTo(CanvasWidth, CanvasHeight , .3f, .3f); tmp.X = scaledJoint.Position.X; tmp.Y = scaledJoint.Position.Y; return tmp; }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            __kinect.Stop();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            __kinect.Stop();
        }

        private void DrawSkeleton()
        {
            SetElementPosition(LeftHandEllipse, handLeft);
        }

        private void SetElementPosition(FrameworkElement element, JPoint jp) 
        { 
            Canvas.SetLeft(element, jp.X - element.Width / 2);
            Canvas.SetTop(element, jp.Y);
        }
    }
}
