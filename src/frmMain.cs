using System;
using System.Windows.Forms;
using OpenCvSharp;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using SExtractorWrapper;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Deconvolvulator
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private string msTitle = @"The DECONVULATOR v1.0";
        private int mnWidth = 0; //Cropped original width to even number of pixels
        private int mnHeight = 0; //Cropped original width to even number of pixels
        private int mnSize = 0; //Number of pixels in cropped image
        private int mnImageDepth = 65536; //16 bit image
        private MatType mImageTypeLoaded = MatType.CV_16U; //Type of input image
        private MatType mImageTypeLoadedChannel = MatType.CV_16U; //Type of each channel of input image
        private MatType mImageTypeInternal = MatType.CV_32F; //Internal representation of image in this program
        private MatType mImageTypeInternalChannel = MatType.CV_32F; //Internal representation of a channel in this program
        private int mnChannels = 1;
        private bool mbImageInLoaded = false;
        private string msImageLoaded = ""; //Currently loaded image
        private bool mbDeblur = true; //False when click "Convolve button"
        private string msRepairedDesription = "";
        private Mat mREPAIRED = new Mat(); //The currently repaired image
        private Mat mREPAIRED_Unstretched = new Mat(); //The currently repaired image (unstretched version)
        private Mat mPsfREPAIRED = new Mat(); //PSF specified to deconvolve the current image with
        private Mat mPsfInvREPAIRED = new Mat(); //PSF of the inverse filter, ie, image could be repaired by convolving with this PSF
        private Mat mFTDisplayREPAIRED = new Mat(); //FT of current repair filter, displayed in picFilterFT
        private float mFWHMREPAIRED = 0.0f; //FWHM for currently repaired image
        private InterpolationFlags interpolationFlags = 0;
        private int mnMaxImageWidth = 300; //Size of Mat stored internally at any one time
        private int mnPercentilesStretchMin = 25; //Out of 10000
        private int mnPercentilesStretchMax = 9990;
        private bool mbDeblurDisplayUI = true;
        private Mat imgToDeblur_Input = new Mat(); //For processing of selected rectangle, store the whole input image
        private bool mbDebug = false; //Output extra debug info into txtDebug

        //For iterative deconvolution algorithms
        private bool mbProcessingCancelled = false;
        private int mnIterationUIFactor = 0;
        private int mnNoOfIterationsStored = 0;
        private int mnNoOfIterationSteps = 10;
        private bool mbMatIterationsStored = false;
        private Mat[][] mDeconvolvedMatIterations = new Mat[4][]; //Stored versions of iterative convolutions, ie Richardson-Lucy after 10 out of 20 iterations

        //History of previously saved repaired images
        private Mat[] mHistory = new Mat[1000];
        private Mat[] mPSFHistory = new Mat[1000];
        private Mat[] mPSFInvHistory = new Mat[1000];
        private Mat[] mFTDisplayHistory = new Mat[1000];
        private float[] mPSFHistoryFWHM = new float[1000];
        private int mHistoryCurrent = 0;
        private bool mbIgnoreCurrentDisplayCboChange = false;

        //Field rotation shared variables
        private float mfFieldRotationCentreX = 0.0f;
        private float mfFieldRotationCentreY = 0.0f;
        private int mnOverlapPixels = 0;

        //UI Event handling
        private bool mbIgnoreZoomChange = false;
        private bool mbIgnoreIterationsChanged = true;
        private bool mbSettingsLoaded = false;

        //Notes
        //Split to avi
        //ffmpeg -i green-500.avi -r 25 -f image2 image-%07d.png
        //Join to avi
        //ffmpeg -start_number 0 -i %d.png -framerate 25 -c:v copy output.avi

        private void frmMain_Load(object sender, EventArgs e)
        {
            cboZoomMode.SelectedIndex = 3;
            mnMaxImageWidth = picOut.Width * 2;
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            mLoadSettings();
            mSetBackColorsForOptions();
            if (!File.Exists("SExtractor.exe") || !File.Exists("SExtractorWrapper.dll"))
            {
                //Star finding not available, hence cannot perform deringing near stars
                chkDeringing.Checked = false;
                chkDeringing.Visible = false;
                udDeringStarThreshold.Visible = false;
                udDeringRepairStrength.Visible = false;
            }
        }

        private bool ImageLoad()
        {
            string sInputFile = txtImage.Text;
            if (sInputFile.StartsWith(@"\"))
                sInputFile = Environment.CurrentDirectory + sInputFile;

            if (!File.Exists(sInputFile))
            {
                txtImage.SelectAll();
                txtImage.Focus();
                this.Cursor = Cursors.Default;
                return false;
            }

            if (!mbImageInLoaded || sInputFile != msImageLoaded)
            {
                //Initialise for a new image
                mHistory = new Mat[1000];
                mPSFHistory = new Mat[1000];
                mPSFInvHistory = new Mat[1000];
                mPSFHistoryFWHM = new float[1000];
                mFTDisplayHistory = new Mat[1000];
                mHistoryCurrent = 0;
                cboHistory.Items.Clear();
                cboCurrent.Items.Clear();
                mbLayersCalc = false;
                mLAYERS = new Mat[6];
                picOut.Location = new System.Drawing.Point(0, 0);
                mbIgnoreZoomChange = true;
                udZoom.Value = 1.0m;
                mbIgnoreZoomChange = false;
                mbLayersCalc = false;

                Mat imgInput = Cv2.ImRead(sInputFile, ImreadModes.Unchanged);
                mnChannels = imgInput.Channels();
                mImageTypeLoaded = imgInput.Type();

                mnImageDepth = 65536;
                if (mImageTypeLoaded == MatType.CV_8U || mImageTypeLoaded == MatType.CV_8UC3 || mImageTypeLoaded == MatType.CV_8UC4) //eg jpg, bmp
                    mnImageDepth = 256;
                else if (mImageTypeLoaded == MatType.CV_32F || mImageTypeLoaded == MatType.CV_32FC3 || mImageTypeLoaded == MatType.CV_32FC4 )
                    mnImageDepth = 65536;

                //Store image as float from 0 to 1
                mImageTypeInternalChannel = MatType.CV_32F;
                if (mnChannels == 1)
                    mImageTypeInternal = MatType.CV_32F;
                else
                    mImageTypeInternal = MatType.CV_32FC3;
   
                imgInput.ConvertTo(imgInput, mImageTypeInternal);
                //Divide by mnImageDepth, unless CV_32F
                if (!(mImageTypeLoaded == MatType.CV_32F || mImageTypeLoaded == MatType.CV_32FC3 || mImageTypeLoaded == MatType.CV_32FC4))
                    imgInput = imgInput / mnImageDepth;

                //Crop image to even width and height
                //If odd, round down to even number below
                mnWidth = imgInput.Cols & -2;
                mnHeight = imgInput.Rows & -2;
                imgInput = new Mat(imgInput, new Rect(0, 0, mnWidth, mnHeight));
                mnSize = mnWidth * mnHeight;

                //Store this original image as the first element in mHistory
                mHistory[0] = imgInput;
                cboHistory.Items.Add("Original");
                cboCurrent.Items.Add("Original");
                cboHistory.SelectedIndex = 0;
                mbIgnoreCurrentDisplayCboChange = true;
                cboCurrent.SelectedIndex = 0;
                mbIgnoreCurrentDisplayCboChange = false;
                mHistoryCurrent = 0;
                mbImageInLoaded = true;
                msImageLoaded = sInputFile;
                mdScale = 1.0m;
                mdLastScale = 1.0m;
                mbIgnoreZoomChange = true;
                udZoom.Value = 1.0m;
                mbIgnoreZoomChange = false;
                optOriginal.Checked = true;
                optPrevious.Visible = false;
                lblLastStored.Visible = false;
                mbROISetPending = false;
                mbROISet = false;

                MatToPictureBox_Zoomed(imgInput, picOut, "Original", true, true, false);

                if (optSharpeningLayers.Checked)
                {
                    mSetUpSharpeningLayers();
                    mLayersChanged();
                }

                this.Text = msTitle + " " + Path.GetFileName(sInputFile) + " " + mImageTypeLoaded.ToString();
            }
            return true;
        }

        //Standard deconvolution using a PSF
        private void btnDeblur_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnDeblur.Enabled = false;

                string sInputFile = txtImage.Text;
                if (!ImageLoad())
                {
                    btnDeblur.Enabled = true;
                    this.Cursor = Cursors.Default;
                    return;
                }

                if (optSharpeningLayers.Checked)
                {
                    //Repair using Sharpening layers logic, ie no deconvolution
                    btnDeblur.Text = "Set up layers"; Application.DoEvents();
                    mSetUpSharpeningLayers();
                    btnDeblur.Text = "Calc layers"; Application.DoEvents();
                    mLayersChanged();
                    btnDeblur.Enabled = true;
                    btnDeblur.Text = "REPAIR"; Application.DoEvents();
                    this.Cursor = Cursors.Default;
                    return;
                }

                mbProcessingCancelled = false;
                mbMatIterationsStored = false;

                int nDeconvolveWidth = mnWidth;
                int nDeconvolveHeight = mnHeight;

                bool bUseROI = mbROISet;
                //Smaller area to process not possible when working with field rotation or want to rotate image
                if (optMotionBlur.Checked && (chkRotateImage.Checked || chkFiledRotationDeblur.Checked))
                    mbROISet = false;

                if (mbROISet)
                {
                    nDeconvolveWidth = mnROIImageWidth;
                    nDeconvolveHeight = mnROIImageHeight;
                }

                //Deconvolution repair parameters
                int nSmoothness = trkBlur.Value;
                bool bConstrainedLeastSquared = optTikhonov.Checked;
                bool bWiener = optWiener.Checked;
                bool bRegularisedInverseFilter = optRIF.Checked;
                bool bLandweber = optLandweber.Checked;
                bool bRichardsonLucy = optLR.Checked;
                bool bCustomRepair = optCustomRepair.Checked;
                double dCLS_Y = double.Parse(txtCLS_Y.Text);
                double nsr = double.Parse(txtNSR.Text);
                int nIterationCount = int.Parse(txtIterations.Text);

                //Type of PSF
                bool bCircularPSF = optCircularBlur.Checked;
                bool bMotionBlur = optMotionBlur.Checked;
                bool bFieldRotationPSF = chkFiledRotationDeblur.Checked;
                if (!bMotionBlur)
                    bFieldRotationPSF = false;
                double dPSFRadius = 0.0d;

                //Circular PSF
                bool bMoffat = optMoffat.Checked;
                bool bGauss = optGaussianDeblur.Checked;
                bool bVoigt = optVoigt.Checked;
                bool bMTF = optMTFPSF.Checked;
                bool bCameraCircleDeblur = optCameraCircleDeblur.Checked;
                double dFWHM = (double)udFWHM.Value;
                float fPSFFWHMActual = 0.0f;
                double dMoffatBeta = (double)udMoffatBeta.Value;
                bool bMoffatInPasses = chkMoffatInPasses.Checked;
                int nMoffatPasses = (int)udMoffatPasses.Value;
                bool bCropPSF = chkCropPSF.Checked;
                double dCropPSF = (double)udCropPSF.Value;
                double dVoigtGaussFraction = (double)udGaussFraction.Value;
                bool bSymetrical = false; //Force PSF to be symetrical for even width/height input images, makes no difference
                dPSFRadius = 5.0 * dFWHM;
                double dWavefrontError = (double)udWave.Value;

                //Motion PSF
                double dMotionLEN = (double)(udMotionBlurLength.Value);
                double dMotionTHETA = (double)(udMotionBlurAngle.Value);
                bool bAntiAlias = chkAntiAliasLine.Checked;
                bool bRotateImage = false;
                if (bMotionBlur)
                {
                    dPSFRadius = dMotionLEN / 2.0d; //For edge repair
                    if (!chkFiledRotationDeblur.Checked)
                        bRotateImage = chkRotateImage.Checked;
                }
                bool bDeringing = chkDeringing.Checked;
                float fDeringingStarThreshold = (float)udDeringStarThreshold.Value;
                float fDeringingRepairStrength = -(float)udDeringRepairStrength.Value;
                if (!bMotionBlur)
                    bDeringing = false; //Do not do deringing unless motion blur

                //Field rotation by transforming to mercator projector, then apply spatially invariant PSF
                float fFieldRotationAngle = (float)udFieldRotationAngle.Value;
                mfFieldRotationCentreX = (float)udCentreFieldRotationX.Value;
                mfFieldRotationCentreY = (float)udCentreFieldRotationY.Value;
                if (udCentreFieldRotationX.Value == -1.0m)
                    mfFieldRotationCentreX = nDeconvolveWidth / 2.0f;
                if (udCentreFieldRotationY.Value == -1.0m)
                    mfFieldRotationCentreY = nDeconvolveHeight / 2.0f;
                float fMaxCornerDistanceFromCentre = MaximumCornerDistanceFromPole_Squared(mfFieldRotationCentreX, mfFieldRotationCentreY, nDeconvolveWidth, nDeconvolveHeight);
                float fFieldRotationLength = (float)(Math.Sqrt(fMaxCornerDistanceFromCentre) * fFieldRotationAngle / 180.0f * Math.PI);
                if (bFieldRotationPSF)
                {
                    chkRepairTopBottom.Checked = false;
                    dPSFRadius = fFieldRotationLength / 2.0f; //For edge repair
                }

                //Field rotation using tiles and spatially variant PSF, not used
                int nTiles = (int)udRotationTiles.Value;
                Mat[,] psfRotation = null;

                //Edge repair parameters
                bool bEdgeTaper = chkRepairEdges.Checked;
                bool bFadeEdges = chkTanhRepair.Checked;
                bool bRepairTopBottom = chkRepairTopBottom.Checked;
                double gamma = double.Parse(txtGamma.Text);
                double beta = double.Parse(txtBeta.Text);

                //Brightness
                double dBrightness = (double)udBrightness.Value;
                if (bLandweber)
                    dBrightness = 1.0d;

                //Feather
                double dFeather = double.Parse(txtFeather.Text);

                Mat imgToDeblur = new Mat();
                if (optOriginal.Checked)
                    imgToDeblur = mHistory[0];
                else if (optPrevious.Checked)
                    imgToDeblur = mHistory[mHistoryCurrent];

                //If area to process defined
                if (mbROISet)
                {
                    imgToDeblur_Input = imgToDeblur.Clone();
                    Rect roi = new Rect(mnROIImageStartX, mnROIImageStartY, mnROIImageWidth, mnROIImageHeight);
                    imgToDeblur = new Mat(imgToDeblur, roi);
                }

                //Process Lum and RGB separately, to increase SNR
                bool bProcessLuminanceAndRGB = chkProcessLuminance.Checked;

                //Up-scale
                Decimal mUpScale = udUpscale.Value;
                int nOrigWidth = nDeconvolveWidth;
                int nOrigHeight = nDeconvolveHeight;
                if (mUpScale != 1.0m)
                {
                    nDeconvolveWidth = (int)Math.Round(imgToDeblur.Width * mUpScale, 0);
                    nDeconvolveHeight = (int)Math.Round(imgToDeblur.Height * mUpScale, 0);
                    Cv2.Resize(imgToDeblur, imgToDeblur, new Size(nDeconvolveWidth, nDeconvolveHeight), 
                        0.0f, 0.0f, InterpolationFlags.Lanczos4);
                    dFWHM =  dFWHM * (float)mUpScale;
                    dPSFRadius = dPSFRadius * (float)mUpScale;
                    dMotionLEN = dMotionLEN * (float)mUpScale;
                }

                if (bRotateImage)
                {
                    btnDeblur.Text = "Rotating"; Application.DoEvents();
                    imgToDeblur = RotateImage(imgToDeblur, -(float)dMotionTHETA,
                        ref nDeconvolveWidth, ref nDeconvolveHeight,
                        true, false, 0, 0, true);
                    dMotionTHETA = 0.0f; //Treat image now as having no motion blur angle
                }

                Mat psf = null;
                btnDeblur.Text = "PSF Calc"; Application.DoEvents();
                psf = PSF(nDeconvolveWidth, nDeconvolveHeight, bCircularPSF,
                    bMoffat, bGauss, bVoigt, bCameraCircleDeblur, bMTF, bMotionBlur, bFieldRotationPSF,
                    dFWHM, ref fPSFFWHMActual, dMoffatBeta, bMoffatInPasses, nMoffatPasses, dVoigtGaussFraction, dWavefrontError,
                    dMotionLEN, dMotionTHETA, bAntiAlias,
                    bCropPSF, dCropPSF, dBrightness, dFeather, bSymetrical, nTiles, fFieldRotationAngle, ref psfRotation,
                    fFieldRotationLength, mfFieldRotationCentreX, mfFieldRotationCentreY);

                Mat filterFTreal = null;
                Mat filterFTimag = null;
                btnDeblur.Text = "Deconv"; Application.DoEvents();
                mREPAIRED = Deblur(nDeconvolveWidth, nDeconvolveHeight, imgToDeblur, psf, sInputFile, bProcessLuminanceAndRGB,
                    bWiener, bRegularisedInverseFilter, bConstrainedLeastSquared, dCLS_Y, bLandweber, bRichardsonLucy, nIterationCount,
                    nSmoothness, bFadeEdges,
                    bRepairTopBottom, mbDeblur, bCustomRepair, bEdgeTaper,
                    nsr, gamma, beta, ref filterFTreal, ref filterFTimag, dPSFRadius,
                    bFieldRotationPSF, nTiles, fFieldRotationAngle, psfRotation, fFieldRotationLength,
                    bDeringing, fDeringingStarThreshold, fDeringingRepairStrength);

                //Downscale again
                if (mUpScale != 1.0m)
                {
                    nDeconvolveWidth = nOrigWidth;
                    nDeconvolveHeight = nOrigHeight;
                    Cv2.Resize(mREPAIRED, mREPAIRED, new Size(nOrigWidth, nOrigHeight),
                        0.0f, 0.0f, InterpolationFlags.Lanczos4);
                    //dFWHM = dFWHM / (float)mUpScale;
                    //dPSFRadius = dPSFRadius / (float)mUpScale;
                    //dMotionLEN = dMotionLEN / (float)mUpScale;
                }

                if (bRotateImage)
                {
                    btnDeblur.Text = "Rotate"; Application.DoEvents();
                    mREPAIRED = RotateImage(mREPAIRED, (float)(udMotionBlurAngle.Value),
                        ref nDeconvolveWidth, ref nDeconvolveHeight, false, true, mnWidth, mnHeight, false);
                }

                //Calculate the PSF that would have been used to convolve, ie InverseFT(1/filterFT)
                if (mbDeblur)
                {
                    if (bLandweber || bRichardsonLucy)
                    {
                        //Calculate filterFT manually using the output image and input image              
                        filterFTreal = CalcFTByDivision(mREPAIRED, imgToDeblur);
                        mPsfInvREPAIRED = PsfFromFT(filterFTreal, null);
                    }
                    else //Using an inverse filter
                        mPsfInvREPAIRED = PsfFromFT(filterFTreal, filterFTimag);
                }

                if (mbROISet)
                {
                    //Copy repaired mREPAIRED over the top of imgToDeblur_Input
                    Mat mSmall = mREPAIRED.Clone();
                    mREPAIRED = imgToDeblur_Input;
                    Rect roi = new Rect(mnROIImageStartX, mnROIImageStartY, mnROIImageWidth, mnROIImageHeight);
                    mSmall.CopyTo(new Mat(mREPAIRED, roi));
                    mSmall.Dispose();
                }

                mREPAIRED_Unstretched = mREPAIRED.Clone();

                if (chkAutostretch.Checked)
                    mREPAIRED = StretchImagePercentiles(mREPAIRED, mnPercentilesStretchMin, mnPercentilesStretchMax);

                if (mbDeblurDisplayUI)
                {
                    //Display Cropped 2D PSF in PictureBox
                    Mat psfDisplay = new Mat();
                    if (chkInvPSF.Checked)
                        psfDisplay = mPsfInvREPAIRED;
                    else
                        psfDisplay = psf;

                    Mat psfCropped = PSFCroppedForDisplay(psfDisplay, (int)(udPSFPlotWidth.Value), 65000.0f);
                    MatToPictureBox(psfCropped, picPSF, false, new System.Drawing.Point(0, 0));
                    psfCropped.Dispose();

                    float fPSFFWHMDisplay = (float)dFWHM;
                    if (bMTF)
                        fPSFFWHMDisplay = fPSFFWHMActual;

                    //Display a profile plot of PSF
                    if (!bMotionBlur && !bFieldRotationPSF)
                    {
                        PlotPSFProfile(psfDisplay, picPSFProfile, (int)udPSFPlotWidth.Value, fPSFFWHMDisplay);
                        PlotMTF(psf, fPSFFWHMDisplay);
                    }

                    //Display the current filter used to repair (A Fourier Transform)
                    if (optFilterFourierTransform.Checked)
                        mDisplayFFTInPic(filterFTreal, picFilterFT, true, 0.0d, (double)udMaxPlotFT.Value);

                    mPsfREPAIRED = psf;
                    mFWHMREPAIRED = fPSFFWHMDisplay;
                    mFTDisplayREPAIRED = filterFTreal;

                    //PSF profile dump to text for Excel etc
                    /*
                    StringBuilder sbOut = new StringBuilder();
                    float PSFCentre = psf.At<float>(psf.Height / 2, psf.Width / 2);
                    float fThreshold = 0.0001f;
                    for (int i = psf.Width / 2; i < psf.Width; i++)
                    {
                        if (i == psf.Width)
                            break;
                        if (psf.At<float>(psf.Height / 2, i) / PSFCentre < fThreshold)
                            break;
                        sbOut.Append((psf.At<float>(psf.Height / 2, i) / PSFCentre).ToString() + "\r\n");
                    }
                    txtPSFDump.Text = sbOut.ToString();
                    */


                    //Display a FT of the input image
                    //Output image greyscale value = log(1 + sqrt( (Real FT)^2 + (Imag FT)^2) )
                    /*
                    frmFourier frm = new frmFourier();
                    Mat displayFT = frm.DisplayFT(InputImage_FTReal * inputImg.Rows * inputImg.Cols,
                        InputImage_FTImag * inputImg.Rows * inputImg.Cols, true);
                    MatToPictureBox(displayFT, picImageFT);
                    */

                    //Decription for PSF parameters and Deconvolve parameters
                    msRepairedDesription = "";
                    if (!mbDeblur)
                        msRepairedDesription = "BLURRED ";
                    else if (optPrevious.Checked)
                        msRepairedDesription = "LAST STORED Image-> " + cboHistory.Items[mHistoryCurrent].ToString() + " ";
                    else
                    {
                        if (bWiener || bRegularisedInverseFilter)
                        {
                            if (bRegularisedInverseFilter)
                                msRepairedDesription = "RIF ";
                            else
                                msRepairedDesription = "Wiener ";
                            msRepairedDesription += "NSR:" + txtNSR.Text + " Blur:" + trkBlur.Value.ToString() + " ";
                        }
                        else if (bConstrainedLeastSquared)
                            msRepairedDesription = "Tikhonov Y:" + txtCLS_Y.Text + " Blur:" + trkBlur.Value.ToString() + " ";
                        else if (bLandweber)
                            msRepairedDesription = "Landweber" + " Iter:" + txtIterations.Text + " Blur:" + trkBlur.Value.ToString() + " ";
                        else if (bRichardsonLucy)
                            msRepairedDesription = "RL" + " Iter:" + txtIterations.Text + " ";
                        else if (optCustomRepair.Checked)
                            msRepairedDesription = "Custom repair ";
                    }

                    if (optMotionBlur.Checked)
                    {
                        if (chkFiledRotationDeblur.Checked)
                        {
                            msRepairedDesription += "Field rotation PSF " + udFieldRotationAngle.Value.ToString() + " deg ";
                            if (udCentreFieldRotationX.Value != -1.0m && udCentreFieldRotationY.Value != -1.0m)
                                msRepairedDesription += "(" + udCentreFieldRotationX.Value.ToString() + "," + udCentreFieldRotationY.Value.ToString() + ") ";
                        }
                        else
                            msRepairedDesription += "Motion Length:" + udMotionBlurLength.Value.ToString("0.0")
                            + " Angle:" + udMotionBlurAngle.Value.ToString("0.0");
                    }
                    else
                    {
                        if (optVoigt.Checked)
                            msRepairedDesription += "Voigt (Gauss fraction: " + dVoigtGaussFraction.ToString("0.00") + " ";
                        else if (optCameraCircleDeblur.Checked)
                            msRepairedDesription += "Camera out of focus ";
                        else if (optGaussianDeblur.Checked)
                            msRepairedDesription += "Gaussian ";
                        else if (optMTFPSF.Checked)
                            msRepairedDesription += "MTF Waves:" + dWavefrontError.ToString("0.00") + " ";
                        else if (optMoffat.Checked)
                        {
                            if (udMoffatBeta.Value == 1.0m)
                                msRepairedDesription += "Lorentz ";
                            else
                                msRepairedDesription += "Moffat Beta:" + dMoffatBeta.ToString("0.00") + " ";
                            if (chkMoffatInPasses.Checked)
                                msRepairedDesription += "Pass cnt:" + nMoffatPasses.ToString("0") + " ";
                        }
                        msRepairedDesription += "FWHM:" + dFWHM.ToString("0.00") + " ";

                        if (udBrightness.Value != 1.0m)
                            msRepairedDesription += "Brightness:" + dBrightness.ToString("0.00") + " ";
                        if (optCircularBlur.Checked)
                        {
                            if (chkCropPSF.Checked)
                                msRepairedDesription += "PSF Reduced at FWHM x:" + udCropPSF.Value.ToString("0.0") + " ";
                        }
                    }

                    if (bDeringing)
                    {
                        msRepairedDesription += " Deringed:" + udDeringStarThreshold.Value.ToString("0.00") + " " +
                             udDeringRepairStrength.Value.ToString("0.000") + " ";
                    }


                    lblLaplacian.Text = "";
                    if (chkLaplacian.Checked)
                    {
                        Mat laplacian = mREPAIRED.EmptyClone(); //Disposed
                        Cv2.Laplacian(mREPAIRED, laplacian, mREPAIRED.Depth());
                        Scalar mu = new Scalar();
                        Scalar sigma = new Scalar();
                        Cv2.MeanStdDev(laplacian, out mu, out sigma);
                        lblLaplacian.Text = (sigma[0]).ToString("0.00");
                        laplacian.Dispose();
                    }

                    MatToPictureBox_Zoomed(mREPAIRED, picOut, msRepairedDesription, true, false, false);
                    if (cboCurrent.Items[cboCurrent.Items.Count - 1].ToString() != "REPAIRED")
                        cboCurrent.Items.Add("REPAIRED");

                    if (cboCurrent.SelectedIndex != cboCurrent.Items.Count - 1)
                    {
                        mbIgnoreCurrentDisplayCboChange = true;
                        cboCurrent.SelectedIndex = cboCurrent.Items.Count - 1;
                        mbIgnoreCurrentDisplayCboChange = false;
                    }

                    if (optPrevious.Checked)
                        cboHistory.SelectedIndex = mHistoryCurrent;

                }

                if (filterFTimag != null)
                    filterFTimag.Dispose();

                GC.Collect();

                mbDeblur = true;

                btnDeblur.Text = "REPAIR";
                Application.DoEvents();
                btnDeblur.Enabled = true;
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                txtDebug.Visible = true;
                lblDebugError.Visible = true;
                txtDebug.Text = "ERROR: btnDeblur_Click " + ex.ToString();
                txtDebug.SelectAll();
                txtDebug.Focus();

                GC.Collect();

                mbDeblur = true;

                btnDeblur.Text = "REPAIR";
                Application.DoEvents();
                btnDeblur.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private Mat Deblur(int nWidth, int nHeight, Mat imgInput, Mat psf, string sImageFile, bool bProcessLuminanceAndRGB,
                bool bWiener, bool bRegularisedInverseFilter, bool bConstrainedLeastSquared, double CLS_Y, bool bLandweber, bool bRichardsonLucy,
                 int nIterationCount, int nBlurQuality, bool bFadeEdges, bool bRepairTopBottom, bool bDeblur,
                 bool bCustomRepair, bool bEdgeTaper, double nsr, double gamma, double beta, 
                 ref Mat filterFTreal, ref Mat filterFTimag,
                 double dPSFRadius, bool bFieldRotationPSF, int nTiles, float fFieldRotationAngle, Mat[,] psfRotation,
                 float fFieldRotationLength, bool bDeringing, float fDeringingStarThreshold, float fDeringingRepairStrength)
        {
            optSharpeningLayers.Checked = false;

            if (bFieldRotationPSF)
            {
                btnDeblur.Text = "Mercator"; Application.DoEvents();
                //Project image to mercator (centred on mfFieldRotationCentreX/Y). The image will now be blurred with a spatially invariant PSF
                Mat mercator = ProjectToMercator(imgInput, mfFieldRotationCentreX, mfFieldRotationCentreY,
                    0.0f, fFieldRotationAngle * 2.0f, ref mnOverlapPixels);

                SaveImage(mercator, @"mercator.tif");

                //Deconvolve
                btnDeblur.Text = "Deconv"; Application.DoEvents();
                Mat deconvolved = Deblur(nWidth, nHeight, mercator, psf, "", bProcessLuminanceAndRGB, 
                             bWiener, bRegularisedInverseFilter, bConstrainedLeastSquared, CLS_Y, bLandweber, bRichardsonLucy,
                             nIterationCount, nBlurQuality, bFadeEdges, bRepairTopBottom, bDeblur,
                              bCustomRepair, bEdgeTaper, nsr, gamma, beta, ref filterFTreal, ref filterFTimag,
                              dPSFRadius, false, nTiles, fFieldRotationAngle, psfRotation, fFieldRotationLength,
                              bDeringing, fDeringingStarThreshold, fDeringingRepairStrength);

                btnDeblur.Text = "Reproject"; Application.DoEvents();
                Mat repaired = MercatorToNormal(deconvolved, imgInput.Width, imgInput.Height,
                    mfFieldRotationCentreX, mfFieldRotationCentreY, mnOverlapPixels);

                deconvolved.Dispose();
                return repaired;
            }

            //Deblur imgInput using psf
            int nNoOfChannelsToProcess = mnChannels;
            if (mnChannels > 1 && bProcessLuminanceAndRGB)
                nNoOfChannelsToProcess = nNoOfChannelsToProcess + 1; //Last channel is for luminance
            Mat[] imgOut = new Mat[nNoOfChannelsToProcess];
            Mat[] imgInOrigChannels = new Mat[mnChannels];
            Mat[] imgIn = new Mat[nNoOfChannelsToProcess];
            Cv2.Split(imgInput, out imgInOrigChannels);
            for (int nChannel = 0; nChannel < mnChannels; nChannel++)
                imgIn[nChannel] = imgInOrigChannels[nChannel];

            if (bProcessLuminanceAndRGB)
            {
                //Last channel set to luminance
                imgIn[nNoOfChannelsToProcess - 1] = new Mat(nHeight, nWidth, imgIn[0].Type(), new Scalar(0.0f));
                //Same as Photoshop Grayscale conversion
                //http://alienryderflex.com/hsp.html
                float fLum = 0.0f;
                for (int x = 0; x < nWidth; x++)
                {
                    for (int y = 0; y < nHeight; y++)
                    {
                        fLum = imgIn[0].At<float>(y,x) * imgIn[0].At<float>(y, x) * 0.299f 
                            + imgIn[1].At<float>(y, x) * imgIn[1].At<float>(y, x) * 0.587f 
                            + imgIn[2].At<float>(y, x) * imgIn[2].At<float>(y, x) * 0.114f;
                        imgIn[nNoOfChannelsToProcess - 1].Set<float>(y, x, (float)Math.Sqrt(fLum));
                    }
                }
            }

            string sCh = "";
            for (int nChannel = 0; nChannel < nNoOfChannelsToProcess; nChannel++)
            {
                if (nChannel == 0 && nNoOfChannelsToProcess > 1)
                    sCh = "B";
                else if (nChannel == 1)
                    sCh = "G";
                else if (nChannel == 2)
                    sCh = "R";
                else if (nChannel == 3)
                    sCh = "L";

                btnDeblur.Text = "Deconv " + sCh; Application.DoEvents();
                int nEdgeRepairMode = 1; //0 is using Tanh, 1 is the technique from SmartDeblur, 
                // ie applying convolution to borders within nPSFRadius pixels of edges
                if (bFadeEdges)
                    nEdgeRepairMode = 0;
                if (bEdgeTaper)
                    imgIn[nChannel] = edgetaper(imgIn[nChannel], gamma, beta, nEdgeRepairMode, dPSFRadius, psf, bRepairTopBottom);


                // Apply filter
                // 1) Constrained least squares (also called Tikhonov) - SINGLE PASS
                // 2) Wiener filter - SINGLE PASS
                // 3) Regularised Inverse Filter - SINGLE PASS 
                // 4) Landweber - ITERATIVE
                // 5) Apply Blur PSF
                // 6) Lucy-Richardson - ITERATIVE

                // Constrained least squares / Tikhonov filtering: Constrained least squares filtering can easily outperform Wiener filtering, 
                // especially in presence of medium and high amounts of noise. In low-noise cases,
                // both the Wiener and the constrained least squares filtering tend to generate very similar results.
                imgOut[nChannel] = new Mat();

                if (!bDeblur)
                {
                    //Apply blurring PSF
                    imgOut[nChannel] = applyPSF(imgIn[nChannel], psf);
                }
                else
                {
                    if (bCustomRepair)
                    {
                        //Do multiple blur operation passes
                        trkBlur.Value = 100;
                        psf = calcPSFCircle(nWidth, nHeight, 1.0d, 0.0d, false, true, 1.0d, true);
                        imgOut[nChannel] = applyTikhonovFilter(imgIn[nChannel], psf, CLS_Y, ref filterFTreal, ref filterFTimag);

                        trkBlur.Value = 1;
                        psf = calcPSFCircle(nWidth, nHeight, 2.0d, 0.0d, false, true, 1.4d, true);
                        imgOut[nChannel] = applyTikhonovFilter(imgOut[nChannel], psf, CLS_Y, ref filterFTreal, ref filterFTimag);

                        psf = PSFCrop(psf, 10.0d, true);

                        //OR Multiple Wiener
                        /*
                        PSF = calcPSFCircle(roi.Size, dRadius/2, 0.0d, false, true, 1.0d,  true, 10.0d);
                        Mat WnrFilter = calcWnrFilter(PSF, nsr);
                        imgOut[nChannel] = applyWienerFilter(imgInF, WnrFilter);
                        imgOut[nChannel] = applyWienerFilter(imgOut[nChannel], WnrFilter);
                         */
                    }
                    //https://github.com/aurelienpierre/Image-Cases-Studies
                    else if (bWiener)
                    {
                        imgOut[nChannel] = applyWienerFilter(imgIn[nChannel], psf, nsr, ref filterFTreal, ref filterFTimag);
                        //imgOut = deconvolutionByWiener__SmartDeblurClone(imgIn[nChannel], psf); //SmartDeblur way using FFTW
                    }
                    else if (bRegularisedInverseFilter)
                    {
                        imgOut[nChannel] = applyInverseLaplacianRegularisedFilter(imgIn[nChannel], psf, nsr, ref filterFTreal, ref filterFTimag);
                    }
                    else if (bConstrainedLeastSquared)
                    {
                        mbMatIterationsStored = true;
                        imgOut[nChannel] = applyTikhonovFilter(imgIn[nChannel], psf, CLS_Y, ref filterFTreal, ref filterFTimag);
                        //imgOut = deconvolutionByTikhonov_SmartDeblurClone(imgIn[nChannel], psf); //SmartDeblur way using FFTW
                    }
                    else if (bLandweber)  //ITERATIONS
                    {
                        //Force blur quality to be zero
                        mbMatIterationsStored = true;
                        imgOut[nChannel] = Landweber_FourierTransform(imgIn[nChannel], psf, nIterationCount, 0, nChannel,  false);
                        //imgOut[nChannel] = Landweber(imgIn[nChannel], psf, nIterationCount, 0, nChannel,  false);
                    }
                    else if (bRichardsonLucy)  //ITERATIONS
                    {
                        float fIgnorePSFLessThan = 0.001f; //Fraction of central maximum
                        float fPSFMax = psf.At<float>(psf.Height / 2, psf.Width / 2);
                        //Set PSF to zero where it is less than fIgnorePSFLessThan / fPSFMax
                        for (int x = 0; x < psf.Width; x++)
                        {
                            for (int y = 0; y < psf.Height; y++)
                            {
                                if (psf.At<float>(y, x) / fPSFMax < fIgnorePSFLessThan)
                                    psf.Set<float>(y, x, 0.0f);
                            }
                        }

                        mbMatIterationsStored = true;
                        imgOut[nChannel] = RichardsonLucyDeconvolve_Filter2D(imgIn[nChannel], psf, nIterationCount,
                            nChannel, false, false);

                        //Identical results using Fourier transforms
                        //mbMatIterationsStored = true;
                        //imgOut[nChannel] = RichardsonLucyDeconvolve_FourierTransform(imgIn[nChannel], psf, nIterationCount, nChannel);

                        //Identical results, but only if psf is seperable, ie for Gaussian ONLY
                        //mbMatIterationsStored = true;
                        //imgOut[nChannel] = RichardsonLucyDeconvolve_SeperableFilter2D(imgIn[nChannel], psf, nIterationCount, nChannel);
                    }
                }
            }

            Mat imgTifOut = new Mat(); //Returned

            if (bDeringing) //Do all channels together at same time in Dering()
            {
                btnDeblur.Text = "Find stars"; Application.DoEvents();
                //For deringing
                int[,] PixelIndicesOfStarsFound = new int[0, 0];
                CExtractorWrapper starExtractor = null;

                //Extract stars only once for RGB images
                int nChannelForStarExtraction = 0;
                if (nNoOfChannelsToProcess > 1)
                    nChannelForStarExtraction = 1; //Choose green in RGB images for star detection

                string sImageBinaryFile = Path.GetFileName(msImageLoaded) + ".bin";
                MatToFloatBinaryFile(imgIn[nChannelForStarExtraction], sImageBinaryFile);
                starExtractor = new CExtractorWrapper(sImageBinaryFile);
                starExtractor.nWidth = imgInput.Width;
                starExtractor.nHeight = imgInput.Height;
                starExtractor.starDetectionThresh = 2.0f;
                starExtractor.bw = 32;
                starExtractor.bh = 32;
                starExtractor.Extract();
                starExtractor.Sort("flux");
                PixelIndicesOfStarsFound = starExtractor.StarIndexMap(true, 0.3f, (float)dPSFRadius * 5.0f, 5.0f);
                if (File.Exists(sImageBinaryFile))
                    File.Delete(sImageBinaryFile);

                btnDeblur.Text = "Dering"; Application.DoEvents();
                imgTifOut = Dering(imgInput.Type(), mnChannels, imgIn, imgOut, fDeringingStarThreshold, fDeringingRepairStrength, (float)dPSFRadius * 2.0f,
                        imgIn[0].Width, imgIn[0].Height, starExtractor, PixelIndicesOfStarsFound);
            }
            else
            {
                imgTifOut = CombineChannels(nWidth, nHeight, mnChannels, imgOut, bProcessLuminanceAndRGB);
            }

            for (int nCh = 0; nCh < nNoOfChannelsToProcess; nCh++)
            {
                imgIn[nCh].Dispose();
                if (nNoOfChannelsToProcess != 1)
                    imgOut[nCh].Dispose();
            }
            GC.Collect();

            return imgTifOut;
        }

        private Mat CombineChannels(int nWidth, int nHeight, int nChannels, Mat[] imgChannels, bool bProcessLuminanceAndRGB)
        {
            //Recombine an array of Mats
            if (mnChannels == 1)
                return imgChannels[0];
            else
            {
                Mat imgTifOut = new Mat(nHeight, nWidth, mImageTypeInternal);
                Cv2.Merge(imgChannels, imgTifOut);
                if (!bProcessLuminanceAndRGB)
                    return imgTifOut;
                else
                {
                    //Combine luminance in last channel with Hue and Sat from imgTifOut
                    float fSat = 0.0f;
                    float fHue = 0.0f;
                    float fV = 0.0f;
                    float fRed = 0.0f;
                    float fGreen = 0.0f;
                    float fBlue = 0.0f;
                    bool bOutOfGammut = false;
                    for (int x = 0; x < nWidth; x++)
                    {
                        for (int y = 0; y < nHeight; y++)
                        {
                            //Get Hue and saturation from deconvolved RGB image
                            RGBtoHSB_PS(imgChannels[0].At<float>(y, x), imgChannels[1].At<float>(y, x),
                                imgChannels[2].At<float>(y, x), ref fHue, ref fSat, ref fV);
                            //Combine this hue and saturation from the luminance (perceived brightness) in imgChannels[3]
                            //Maintain perceived brightness, which is the same as fLum in a grayscale
                            HSPtoRGB(fHue, fSat, imgChannels[3].At<float>(y, x), 
                                ref fRed, ref fGreen, ref fBlue, ref bOutOfGammut, false, 1);
                            //Set the RGB channels to the new R G and B
                            imgChannels[0].Set<float>(y, x, fRed);
                            imgChannels[1].Set<float>(y, x, fGreen);
                            imgChannels[2].Set<float>(y, x, fBlue);
                        }
                    }
                    Mat[] imgChannelsRGB = new Mat[3];
                    imgChannelsRGB[0] = imgChannels[0];
                    imgChannelsRGB[1] = imgChannels[1];
                    imgChannelsRGB[2] = imgChannels[2];
                    Cv2.Merge(imgChannelsRGB, imgTifOut);
                    return imgTifOut;
                }
            }
        }

        //Deringing
        private Mat Dering(MatType origType, int nNoOfChannels, Mat[] imgOrigA, Mat[] imgRepairedA, float fDeringingStarThreshold, float fDeringingRepairStrength, float fMotionBlurLength,
            int nWidth, int nHeight, CExtractorWrapper starExtractor, int[,] PixelIndicesOfStarsFound)
        {
            //For motion blur only
            float fPeriod = fMotionBlurLength * 1.07f; //Sync function artifacts pattern caused by star 
            // edges seems to have a period 7% higher than the length of motion blur
            int nPeriodRounded = (int)Math.Round(fPeriod);
            int nHalfPeriod = (int)Math.Round(fPeriod / 2.0f, 0);

            //fDeringingStarThreshold = Star centre threshold above local (Gaussian blurred) background
            //fDeringingRepairStrength = Repair if repaired minus moving average minimum is less than this
            Mat restored = new Mat();

            Mat imgOrig = new Mat();
            Mat imgRepaired = new Mat();
            if (nNoOfChannels == 1)
            {
                imgOrig = imgOrigA[0];
                imgRepaired = imgRepairedA[0];
            }
            else
            {
                imgOrig = imgOrigA[1]; //Do analysis on green layer, than apply same repairs to all
                imgRepaired = imgRepairedA[1];
            }

            //Original 
            float[,] aImgOrig = new float[nHeight, nWidth];
            imgOrig.GetArray(0, 0, aImgOrig);

            float[,] aImgOrigR = new float[0, 0];
            float[,] aImgOrigB = new float[0, 0];
            if (nNoOfChannels > 1) //Get additional arrays for extra channels, only used at final repair stage
                                   //ie not used for analysis of where repairs should be made
            {
                aImgOrigR = new float[nHeight, nWidth];
                aImgOrigB = new float[nHeight, nWidth];
                imgOrigA[0].GetArray(0, 0, aImgOrigR);
                imgOrigA[2].GetArray(0, 0, aImgOrigB);
            }

            //Blurred version of original
            Mat mBlurredOrig = imgOrig.EmptyClone(); //Disposed
            Cv2.GaussianBlur(imgOrig, mBlurredOrig, new Size(0, 0), nPeriodRounded);
            //Original minus a blurred version of itself
            Mat mOrigAboveBackground = imgOrig - mBlurredOrig; //Disposed
            float[,] aOrigAboveBackground = new float[nHeight, nWidth];
            mOrigAboveBackground.GetArray(0, 0, aOrigAboveBackground);
            mBlurredOrig.Dispose();
            mOrigAboveBackground.Dispose();
            GC.Collect();
            //SaveImage(mOrigAboveBackground, @"F:\temp\mask\rotated_IC434_Stretched_motion_Bkg.tif");

            //Repaired (with artifacts)
            float[,] aRepaired = new float[nHeight, nWidth];
            imgRepaired.GetArray(0, 0, aRepaired);

            //Restored array (initially the same as Repaired
            float[,] aRestored = new float[nHeight, nWidth];
            imgRepaired.GetArray(0, 0, aRestored);
            float[,] aRestoredR = new float[0, 0];
            float[,] aRestoredB = new float[0, 0];
            if (nNoOfChannels > 1) //Get red and blue channels into extra arrays
            {
                aRestoredR = new float[nHeight, nWidth];
                imgRepairedA[0].GetArray(0, 0, aRestoredR);
                aRestoredB = new float[nHeight, nWidth];
                imgRepairedA[2].GetArray(0, 0, aRestoredB);
            }

            //Blurred version of repaired
            Mat mBlurred = imgRepaired.EmptyClone(); //Disposed;
            Cv2.GaussianBlur(imgRepaired, mBlurred, new Size(0, 0), nPeriodRounded);
            //Repaired minus a blurred version of itself
            Mat mAboveBackground = imgRepaired - mBlurred; //Disposed
            float[,] aAboveBackground = new float[nHeight, nWidth];
            mAboveBackground.GetArray(0, 0, aAboveBackground);
            mBlurred.Dispose();
            mAboveBackground.Dispose();
            GC.Collect();

            float fSum = 0.0f;
            int nPts = 0;
            ArrayList[] alLocalMax = new ArrayList[nHeight];
            bool[,] abLocalStarMask = new bool[nHeight, nWidth]; //Co-ords close to star maxima (within nPeriodRounded)
            ArrayList alMinXOffsets = new ArrayList();
            ArrayList alMinValues = new ArrayList();
            ArrayList alMinYValues = new ArrayList();
            ArrayList alMinStarXCoord = new ArrayList();
            ArrayList alStarIndex = new ArrayList();

            float[,] aRollingAvg = new float[nHeight, nWidth];
            float[,] aRollingAvg_Diff = new float[nHeight, nWidth];

            for (int y = 0; y < nHeight; y++)
            {
                alLocalMax[y] = new ArrayList();
                fSum = 0.0f;
                nPts = 0;

                //Calculate a rolling average along each row
                //1) First nPeriodRounded points
                for (int x = 0; x < nPeriodRounded; x++)
                {
                    fSum = fSum + aRepaired[y, x];
                    nPts++;
                }
                for (int x = 0; x < nHalfPeriod; x++)
                {
                    aRollingAvg[y, x] = fSum / nPts;
                }
                //2) Main section from nHalfPeriod to nWidth - nHalfPeriod
                for (int x = nHalfPeriod; x < nWidth - nHalfPeriod; x++)
                {
                    fSum = fSum + aRepaired[y, x + nHalfPeriod] - aRepaired[y, x - nHalfPeriod];
                    aRollingAvg[y, x] = fSum / nPts;
                }
                //3) Last nHalfPeriod points
                fSum = 0.0f;
                nPts = 0;
                for (int x = nWidth - nHalfPeriod; x < nWidth; x++)
                {
                    fSum = fSum + aRepaired[y, x];
                    nPts++;
                }
                for (int x = nWidth - nHalfPeriod; x < nWidth; x++)
                {
                    aRollingAvg[y, x] = fSum / nPts;
                }
                //Repaired minus Rolling average
                for (int x = 0; x < nWidth; x++)
                {
                    aRollingAvg_Diff[y, x] = aRepaired[y, x] - aRollingAvg[y, x];
                }
                //End rolling average
            }

            /*
            for (int y = 0; y < nHeight; y++)
            {
                //Store local maximum points of stars
                //Finding star maximum using algorithm where central max greater than 2 pixels before and 
                // greater than two pixels after
                /*
                for (int x = 2; x < nWidth - 2; x++)
                {
                    if (aAboveBackground[y, x] > fDeringingStarThreshold)
                    {
                        if (aAboveBackground[y, x] >= aAboveBackground[y, x - 1] && aAboveBackground[y, x - 1] >= aAboveBackground[y, x - 2]
                                && aAboveBackground[y, x] >= aAboveBackground[y, x + 1] && aAboveBackground[y, x] >= aAboveBackground[y, x + 2])
                        {
                            alLocalMax[y].Add(x);
                            for (int m = x - nPeriodRounded; m < x + nPeriodRounded; m++)
                            {
                                if (m >= 0 && m < nWidth)
                                    abLocalStarMask[y, m] = true;
                            }
                        }
                    }
                }
            }
            */

            //Find star streak areas which are expected length (between 0.7 and 1.5 x nPeriodRounded)
            // and which are more than two rows tall
            /*
            int nXStart = 0;
            int nYStart = 0;
            int nXEnd = 0;
            int nYEnd = 0;
            int nPtsAddedInRow = 0;
            for (int y = 0; y < nHeight; y++)
            {
                for (int x = 0; x < nWidth; x++)
                {
                    if (abLocalStarMask[y, x])
                        continue; //Pixel already in a star rectangle
                    if (aOrigAboveBackground[y, x] >= 0.0f)
                    {
                        nXStart = x;
                        nXEnd = x;
                        nYStart = y;
                        nYEnd = y;
                        //Moving down rows
                        for (int j = y; j < nHeight; j++)
                        {
                            if (j - nYStart > 10)
                            {
                                //Too many rows, this is probably not a star
                                nYEnd = nYStart;
                                break;
                            }
                            nPtsAddedInRow = 0;
                            //Expand current row to the right.
                            for (int i = nXStart; i < nWidth; i++)
                            {
                                if (abLocalStarMask[j, i])
                                    break;
                                if (aOrigAboveBackground[j, i] < 0.0f && i >= nXEnd)
                                    break;
                                if (i - nXStart > nPeriodRounded * 1.5) //Streak too long, stop
                                    break;
                                if (i > nXEnd)
                                    nXEnd = i;
                                if (aOrigAboveBackground[j, i] >= 0.0f)
                                    nPtsAddedInRow++;
                            }
                            //Expand current row to the left.
                            for (int i = nXStart - 1; i >= 0; i--)
                            {
                                if (abLocalStarMask[j, i])
                                    break;
                                if (aOrigAboveBackground[j, i] < 0.0f)
                                    break;
                                if (i < nXStart)
                                    nXStart = i;
                                nPtsAddedInRow++;
                            }
                            if (nPtsAddedInRow == 0)
                                break;
                            nYEnd = j;
                        }
                    }
                    //Star streak now detected within rectangle (nXStart, nYStart) to (nXEnd, nYEnd)
                    //Check the rectangle is long enough and tall enough
                    if (nXEnd - nXStart > 0.7 * nPeriodRounded)
                    {
                        //Calculate centroid position and mark abLocalStarMask as star present
                        float fMomentX = 0.0f;
                        float fMomentY = 0.0f;
                        float fSumX = 0.0f;
                        float fSumY = 0.0f;
                        for (int i = nXStart; i <= nXEnd; i++)
                        {
                            for (int j = nYStart; j <= nYEnd; j++)
                            {
                                if (aOrigAboveBackground[j, i] >= 0.0f)
                                {
                                    abLocalStarMask[j, i] = true;

                                    fMomentX = fMomentX + aOrigAboveBackground[j, i] * (i - nXStart);
                                    fSumX = fSumX + aOrigAboveBackground[j, i];

                                    fMomentY = fMomentY + aOrigAboveBackground[j, i] * (j - nYStart);
                                    fSumY = fSumY + aOrigAboveBackground[j, i];
                                }
                            }
                        }
                        int nXCentroid = (int)Math.Round(fMomentX / fSumX, 0) + nXStart;
                        int nYCentroid = (int)Math.Round(fMomentY / fSumY, 0) + nYStart;

                        //Add the X centroid to alLocalMax for each row the star streak is on
                        for (int j = nYStart; j <= nYEnd; j++)
                        {
                            alLocalMax[j].Add(nXCentroid);
                        }
                    }
                }
            }
            */

            //Find areas of rows which are candiates for repair (moving out from the row star maxima)
            ArrayList alOffsets = new ArrayList();
            ArrayList alVals = new ArrayList();
            bool bCanidateForRepair = false;
            int nOffset = 0;
            int nLastMinimaXPos = 0;
            int nStarIndex = 0;
            int nStarCentreX = 0;
            float fStarPeak = 0.0f;

            for (int i = 0; i < starExtractor.nobj; i++)
            {
                nStarIndex = starExtractor.SortedIndices[i];

                if (starExtractor.NonStellar[nStarIndex])
                    continue;

                nStarCentreX = (int)Math.Round(starExtractor.x[nStarIndex], 0);
                fStarPeak = starExtractor.peak[nStarIndex];

                //If shape is typical of an area of nebulosity, remove area from PixelIndicesOfStarsFound array

                if (fStarPeak < fDeringingStarThreshold)
                    continue;


                for (int y = starExtractor.ymin[nStarIndex]; y < starExtractor.ymax[nStarIndex]; y++)
                {
                    //Find local minima in aRollingAvg_Diff either side of star centre on this row
                    alOffsets = new ArrayList();
                    alVals = new ArrayList();

                    //Go to the left of the maximum point, then to the right
                    for (int nIncrement = -1; nIncrement < 2; nIncrement = nIncrement + 2)
                    {
                        nLastMinimaXPos = 0;
                        for (int nX = nStarCentreX; nX > 2 && nX < nWidth - 2; nX = nX + nIncrement)
                        {
                            if (Math.Abs((nX - nStarCentreX) - nLastMinimaXPos) > nPeriodRounded * 3)
                                break;
                            if (aRollingAvg_Diff[y, nX] < fDeringingRepairStrength)
                            {
                                if (aRollingAvg_Diff[y, nX] < aRollingAvg_Diff[y, nX + 1] && aRollingAvg_Diff[y, nX] < aRollingAvg_Diff[y, nX + 2] &&
                                    aRollingAvg_Diff[y, nX] < aRollingAvg_Diff[y, nX - 1] && aRollingAvg_Diff[y, nX] < aRollingAvg_Diff[y, nX - 2])
                                {
                                    alOffsets.Add(nX - nStarCentreX);
                                    alVals.Add(aRollingAvg_Diff[y, nX]);
                                    nLastMinimaXPos = nX - nStarCentreX;
                                }
                            }
                        }
                    }

                    if (alOffsets.Count > 0)
                    {
                        //Is there an acceptable minimum value close to a position +1 or +2 or -1 or -2 x nPeriod
                        bCanidateForRepair = false;
                        nOffset = 0;
                        for (int l = 0; l < alOffsets.Count; l++)
                        {
                            if ((float)alVals[l] < fDeringingRepairStrength)
                            {
                                nOffset = (int)alOffsets[l];
                                if (Math.Abs(nOffset - nPeriodRounded) < nPeriodRounded * 0.25f)
                                    bCanidateForRepair = true;
                                else if (Math.Abs(nOffset - 2 * nPeriodRounded) < nPeriodRounded * 0.25f)
                                    bCanidateForRepair = true;
                                else if (Math.Abs(nOffset + nPeriodRounded) < nPeriodRounded * 0.25f)
                                    bCanidateForRepair = true;
                                else if (Math.Abs(nOffset + 2 * nPeriodRounded) < nPeriodRounded * 0.25f)
                                    bCanidateForRepair = true;
                                if (bCanidateForRepair)
                                    break;
                            }
                        }

                        if (bCanidateForRepair)
                        {
                            alMinXOffsets.Add(alOffsets.ToArray());
                            alMinValues.Add(alVals.ToArray());
                            alMinYValues.Add(y);
                            alMinStarXCoord.Add(nStarCentreX);
                            alStarIndex.Add(nStarIndex);
                        }
                    }
                }
            }

            //Consider repair cadidate rows and do repairs out to fDeringingRepairStrength, store in aRestored array
            object[] aMinXOffsets = new object[0];
            object[] aMinValues = new object[0];
            int nRowYCoord = 0;
            int nOffsetAbs = 0;
            int nOffsetLast = 0;
            bool bDoRepair = false;
            float fMinValue = 0.0f;
            float fRepairFactor = 0.0f;
            int nIndexWhereSignChanges = 0;
            int nStartOffsetArrayIndexStart = 0;
            int nEndOffsetArrayIndexEnd = 0;
            int nRepairPosX = 0;
            int nStarMaxXCoord = 0;

            for (int i = 0; i < alMinXOffsets.Count; i++)
            {
                aMinXOffsets = (object[])alMinXOffsets[i];
                aMinValues = (object[])alMinValues[i];
                nStarMaxXCoord = (int)alMinStarXCoord[i];
                nRowYCoord = (int)alMinYValues[i];

                //if (nRowYCoord == 697)
                //    nRowYCoord = 697;

                nOffset = 0;
                nOffsetAbs = 0;
                nOffsetLast = 0;
                bDoRepair = false;
                fMinValue = 0.0f;
                fRepairFactor = 0.0f;

                nIndexWhereSignChanges = aMinXOffsets.Length;
                nOffsetLast = (int)aMinXOffsets[0];
                for (int l = 1; l < aMinXOffsets.Length; l++)
                {
                    nOffset = (int)aMinXOffsets[l];
                    if (Math.Sign(nOffset) != Math.Sign(nOffsetLast))
                    {
                        nIndexWhereSignChanges = l;
                        break;
                    }
                    nOffsetLast = nOffset;
                }

                nStartOffsetArrayIndexStart = 0;
                nEndOffsetArrayIndexEnd = 0;
                for (int nIncrement = -1; nIncrement < 2; nIncrement = nIncrement + 2)
                {
                    if (nIncrement == -1)
                    {
                        nStartOffsetArrayIndexStart = 0;
                        nEndOffsetArrayIndexEnd = nIndexWhereSignChanges;
                    }
                    else
                    {
                        nStartOffsetArrayIndexStart = nIndexWhereSignChanges;
                        nEndOffsetArrayIndexEnd = aMinXOffsets.Length;
                    }

                    nOffsetLast = 0;
                    bDoRepair = false;
                    for (int l = nStartOffsetArrayIndexStart; l < nEndOffsetArrayIndexEnd; l++)
                    {
                        nOffset = (int)aMinXOffsets[l];
                        nOffsetAbs = Math.Abs(nOffset);
                        fMinValue = (float)aMinValues[l];

                        if ((nOffsetAbs - nOffsetLast) > nPeriodRounded * 1.5) //To long since last minimum
                            bDoRepair = true;
                        else if ((nOffsetAbs - nOffsetLast) > nPeriodRounded / 1.3)
                        {
                            if (fMinValue < fDeringingRepairStrength)
                                nOffsetLast = nOffsetAbs; //Keep going
                            else
                                bDoRepair = true; //Minimum is at correct position, but below threshold to repair
                        }

                        nRepairPosX = 0;
                        if (bDoRepair || l == nEndOffsetArrayIndexEnd - 1)
                        {
                            if (nOffsetLast != 0)
                            {
                                //Repair only up to nOffsetLast
                                for (int x = 0; x < nOffsetLast + nHalfPeriod; x++) //Repair half a period beyond the last repair pos
                                {
                                    nRepairPosX = nStarMaxXCoord + x * nIncrement;
                                    if (nRepairPosX < 0 || nRepairPosX >= nWidth)
                                        break;

                                    //Do not repair if come within one period of another star, unless close to current star
                                    if (PixelIndicesOfStarsFound[nRowYCoord, nRepairPosX] > 0 && x > nPeriodRounded * 2)
                                        continue;

                                    //Do not repair intial section near star
                                    // and taper the repair for the next section close to the star
                                    if (x < nHalfPeriod * 1.2f)
                                        fRepairFactor = 0.0f;
                                    else if (x < nHalfPeriod * 1.4f)
                                        fRepairFactor = (x - nHalfPeriod * 1.2f) / (nHalfPeriod * 0.2f);
                                    else
                                        fRepairFactor = 1.0f;

                                    if (fRepairFactor > 0.0f)
                                    {
                                        aRestored[nRowYCoord, nRepairPosX] = aImgOrig[nRowYCoord, nRepairPosX] * fRepairFactor +
                                            aRestored[nRowYCoord, nRepairPosX] * (1.0f - fRepairFactor);
                                        if (nNoOfChannels > 1)
                                        {
                                            //Do same repair in R and B channel
                                            aRestoredR[nRowYCoord, nRepairPosX] = aImgOrigR[nRowYCoord, nRepairPosX] * fRepairFactor +
                                                    aRestoredR[nRowYCoord, nRepairPosX] * (1.0f - fRepairFactor);
                                            aRestoredB[nRowYCoord, nRepairPosX] = aImgOrigB[nRowYCoord, nRepairPosX] * fRepairFactor +
                                                    aRestoredB[nRowYCoord, nRepairPosX] * (1.0f - fRepairFactor);
                                        }
                                    }
                                }

                            }
                            break; //Repair finished
                        }

                    }
                }

            }

            if (nNoOfChannels == 1)
                restored = new Mat(nHeight, nWidth, origType, aRestored);
            else
            {
                restored = new Mat(nHeight, nWidth, origType); 
                Mat[] restoredA = new Mat[mnChannels];
                restoredA[0] = new Mat(nHeight, nWidth, imgOrigA[0].Type(), aRestoredR);
                restoredA[1] = new Mat(nHeight, nWidth, imgOrigA[1].Type(), aRestored);
                restoredA[2] = new Mat(nHeight, nWidth, imgOrigA[2].Type(), aRestoredB);
                Cv2.Merge(restoredA, restored);
                for (int nCh = 0; nCh < mnChannels; nCh++)
                {
                    restoredA[nCh].Dispose();
                }
            }
            GC.Collect();


            return restored;
        }

        //Star extraction support
        private void MatToFloatBinaryFile(Mat m, string sOutputFileName)
        {
            int nWidth = m.Width;
            int nHeight = m.Height;

            float[,] aImg = new float[nHeight, nWidth];
            m.GetArray(0, 0, aImg);

            byte[] bImgFloatData = new Byte[nHeight * nWidth * 4];
            int nPixelNo = 0;
            for (int y = 0; y < nHeight; y++)
            {
                for (int x = 0; x < nWidth; x++)
                {
                    byte[] vOut = BitConverter.GetBytes(aImg[y, x]);

                    bImgFloatData[4 * nPixelNo] = vOut[0];
                    bImgFloatData[4 * nPixelNo + 1] = vOut[1];
                    bImgFloatData[4 * nPixelNo + 2] = vOut[2];
                    bImgFloatData[4 * nPixelNo + 3] = vOut[3];

                    nPixelNo++;
                }
            }

            File.WriteAllBytes(sOutputFileName, bImgFloatData);
        }

        private void MatToUShortBinaryFile(Mat m, string sOutputFileName)
        {
            int nWidth = m.Width;
            int nHeight = m.Height;

            ushort[] imgData = new ushort[nWidth * nHeight];
            float[,] aImg = new float[nHeight, nWidth];
            m.GetArray(0, 0, aImg);

            ushort uData = 0;
            for (int y = 0; y < nHeight; y++)
            {
                for (int x = 0; x < nWidth; x++)
                {
                    imgData[y * mnWidth + x] = (ushort)Math.Round(aImg[y, x] * 65536, 0);
                }
            }
            byte[] bImgData = new Byte[imgData.Length * 2];
            for (int i = 0; i < imgData.Length; i++)
            {
                bImgData[2 * i] = (Byte)(imgData[i] / 256);
                bImgData[2 * i + 1] = (Byte)(imgData[i] % 256);
            }

            File.WriteAllBytes(sOutputFileName, bImgData);
        }

        //Construct a PSF
        private Mat PSF(int nWidth, int nHeight,
                    bool bCircularPSF, bool bMoffat, bool bGauss, bool bVoigt, 
                    bool bCameraCircleDeblur, bool bMTF, bool bMotionBlur, bool bFieldRotationPSF,
                    double dFWHM, ref float fPSFFWHMActual , double dMoffatBeta, bool bMoffatInPasses, int nMoffatPasses,
                    double dVoigtGaussFraction, double dWave,
                    double LEN, double THETA, bool bAntiAlias,
                    bool bCropPSF, double dCropPSFAtRadius, double dBrightness, double dFeather, bool bSymetrical,
                    int nTiles, float fFieldRotationAngle, ref Mat[,] psfRotation, float fFieldRotationLength, 
                    float fFieldRotationPoleX, float fFieldRotationPoleY)
        {
            //Make PSF
            Mat PSF = null;
            double dPSFRadius = 0.0d;
            double dAdjustPSFRadiusFactor = 1.0d;

            if (bCircularPSF)
            {
                if (bMoffat)
                    dPSFRadius = dFWHM / 2.35 / 0.707;
                else if (bGauss)
                    dPSFRadius = dFWHM / 2.35 / 0.707;
                else if (bCameraCircleDeblur)
                    dPSFRadius = dFWHM / 2.0d;
                else if (bVoigt)
                {
                    //https://en.wikipedia.org/wiki/Voigt_profile
                    if (dVoigtGaussFraction != 1.0d)
                        dVoigtGaussFraction = dVoigtGaussFraction / (1.0d - dVoigtGaussFraction);
                    else
                        dVoigtGaussFraction = 1000.0d;
                    dAdjustPSFRadiusFactor = 1.0d / (0.5346 + Math.Sqrt(0.2166 + dVoigtGaussFraction * dVoigtGaussFraction));
                    dPSFRadius = dFWHM / 2.35 / 0.707;
                }

                if (bMoffat && bMoffatInPasses)
                {
                    int nRepeats = nMoffatPasses;
                    Mat[] PSFRepeats = new Mat[nRepeats];
                    for (int i = 0; i < nRepeats; i++)
                    {
                        PSFRepeats[i] = calcPSFCircle(nWidth, nHeight, dPSFRadius / nRepeats, dFeather, bGauss, bMoffat,
                            dMoffatBeta, bSymetrical);
                        if (i == 0)
                            PSF = PSFRepeats[0];
                        else
                            PSF = Convolve(PSF, PSFRepeats[i], true);
                    }
                }
                else if (bGauss || bMoffat || bCameraCircleDeblur) //Normal moffat or gauss
                {
                    PSF = calcPSFCircle(nWidth, nHeight, dPSFRadius, dFeather, bGauss, bMoffat,
                        dMoffatBeta, bSymetrical);
                }
                else if (bMTF)
                {
                    PSF = calcPSFFromMTF(nWidth, nHeight, dFWHM, dWave, ref dPSFRadius);
                }
                else if (bVoigt)
                {
                    //Combine two or more PSFs
                    if (dVoigtGaussFraction == 0.0d)
                        PSF = calcPSFCircle(nWidth, nHeight, dPSFRadius, dFeather, false, true, 1.0d,  bSymetrical);
                    else if (dVoigtGaussFraction == 1000.0d)
                        PSF = calcPSFCircle(nWidth, nHeight, dPSFRadius, dFeather, true, false, 0.0d,  bSymetrical);
                    else
                    {
                        Mat PSF_G = calcPSFCircle(nWidth, nHeight, dPSFRadius * dAdjustPSFRadiusFactor * dVoigtGaussFraction,
                            dFeather, true, false, 0.0d, bSymetrical); //Gaussian
                        Mat PSF2_L = calcPSFCircle(nWidth, nHeight, dPSFRadius * dAdjustPSFRadiusFactor,
                            dFeather, false, true, 1.0d,  bSymetrical); //Lorentz

                        double dFWHM_Lorentz = dPSFRadius * dAdjustPSFRadiusFactor * 2.35 * 0.707;
                        double dFWHM_Gauss = dPSFRadius * dAdjustPSFRadiusFactor * dVoigtGaussFraction * 2.35 * 0.707;
                        double dFWHMCombined = 0.5346 * dFWHM_Lorentz + Math.Sqrt(0.2166 * dFWHM_Lorentz * dFWHM_Lorentz + dFWHM_Gauss * dFWHM_Gauss);

                        PSF = Convolve(PSF_G, PSF2_L, false); //Convolve order doesn't matter
                    }

                }

                if (bCropPSF)
                    PSF = PSFCrop(PSF, dFWHM * dCropPSFAtRadius, false);

                //Normalise and apply brigtness to cicular PSFs
                Scalar summa = Cv2.Sum(PSF);
                PSF = PSF / summa[0] / dBrightness;
            }
            else if (bFieldRotationPSF)
            {
                //Constant PSF
                //Determine size of mercator projection
                int nOutHeight = 0;
                int nOutWidth = 0;
                int nOutWidthExtended = 0;
                int nOverlapPixels = 0;
                MercatorSize(nWidth, nHeight, ref nOutWidth, ref nOutWidthExtended, ref nOutHeight,
                    fFieldRotationPoleX, fFieldRotationPoleY, 0.0f, fFieldRotationAngle * 2.0f, ref nOverlapPixels);

                PSF = calcPSFMotionBlur(nOutWidthExtended, nOutHeight, fFieldRotationLength, 0.0f, bAntiAlias, dFeather);

                /*
                //Spatially varying PSF
                //Make array of PSFs
                float fCentreX = nWidth / 2.0f;
                float fCentreY = nHeight / 2.0f;
                float fTileCentreX = 0.0f;
                float fTileCentreY = 0.0f;
                double dLength = 0.0d;
                double dTheta = 0.0d;
                double dFieldRotationAngleRad = fFieldRotationAngle/180.0d*Math.PI;

                int nTilesX = nTiles;
                int nTilesY = nTiles * nHeight / nWidth;
                if (nTilesY < 2)
                    nTilesY = 2;

                int nTileWidth = nWidth / nTilesX;
                if (nTileWidth % 2 == 1)
                    nTileWidth = nTileWidth - 1;

                int nTileHeight = nHeight / nTilesY;
                if (nTileHeight % 2 == 1)
                    nTileHeight = nTileHeight - 1;

                int nTileLastWidth = nWidth - nTileWidth * (nTilesX - 1);
                int nTileLastHeight = nHeight - nTileHeight * (nTilesY - 1);

                int nCurrentTileWidth = 0;
                int nCurrentTileHeight = 0;

                int nOverlap = OverlapLength(nWidth, nHeight, fFieldRotationAngle, nTilesX);

                psfRotation = new Mat[nTilesX, nTilesY];

                for (int xTileNo = 0; xTileNo < nTilesX; xTileNo++)
                {
                    for (int yTileNo = 0; yTileNo < nTilesY; yTileNo++)
                    {
                        if (xTileNo == nTilesX - 1)
                            fTileCentreX = xTileNo * nTileWidth + nTileLastWidth / 2;
                        else
                            fTileCentreX = xTileNo * nTileWidth + nTileWidth / 2;
                        if (yTileNo == nTilesY - 1)
                            fTileCentreY = yTileNo * nTileHeight + nTileLastHeight / 2;
                        else
                            fTileCentreY = yTileNo * nTileHeight + nTileHeight / 2;

                        if (xTileNo == nTilesX - 1)
                            nCurrentTileWidth = nTileLastWidth + nOverlap;
                        else if (xTileNo == 0)
                            nCurrentTileWidth = nTileWidth + nOverlap;
                        else
                            nCurrentTileWidth = nTileWidth + 2*nOverlap;

                        if (yTileNo == nTilesY - 1)
                            nCurrentTileHeight = nTileLastHeight + nOverlap;
                        else if (yTileNo == 0)
                            nCurrentTileHeight = nTileHeight + nOverlap;
                        else
                            nCurrentTileHeight = nTileHeight + 2 * nOverlap;

                        dLength = dFieldRotationAngleRad * Math.Sqrt((fTileCentreX - fCentreX) * (fTileCentreX - fCentreX) +
                             (fTileCentreY - fCentreY) * (fTileCentreY - fCentreY));
                        dTheta = Math.Atan((fTileCentreX - fCentreX) / (fTileCentreY - fCentreY)) * 180.0d / Math.PI;
                        psfRotation[xTileNo, yTileNo] = calcPSFMotionBlur(nCurrentTileWidth, nCurrentTileHeight, dLength, dTheta, bAntiAlias, dFeather);
                    }
                }

                //Return top left tile PSF
                PSF = psfRotation[0, 0].Clone();
                */

            }
            else if (bMotionBlur)
            {
                PSF = calcPSFMotionBlur(nWidth, nHeight, LEN, THETA, bAntiAlias, dFeather);
                dPSFRadius = LEN / 2.0d;
            }

            /*
            Mat PSF1 = calcPSFCircle(nWidth, nHeight, 4.7d, 0.0d, true, false, 0.0d, bSymetrical);                //Gaussian
            Mat PSF2 = calcPSFCircle(nWidth, nHeight, dFWHM / 2.35 / 0.707, 0.0d, false, true, 1.0d, bSymetrical); //Lorentz
            PSF = Convolve(PSF1, PSF2, true); //Order doesn't matter
            if (bCropPSF)
                PSF = PSFCrop(PSF, dFWHM * dCropPSFAtRadius, true);
            */

            fPSFFWHMActual = (float)dPSFRadius;

            return PSF;
        }

        //Colour processing and conversions
        public void HSPtoRGB(float H, float S, float P,
         ref float R, ref float G, ref float B, ref bool bOutOfGamut,
            bool bFixOutOfGamut, int nMode)
        {
            //Call to HSP to RGB
            HSPtoRGB_Int(H, S, P, ref R, ref G, ref B, ref bOutOfGamut, nMode);

            //But, if RGB is out of gamma, make adjustments
            //Reduce dSat just enough so that not out of gamut, keeping Hue the same
            if (bOutOfGamut && bFixOutOfGamut)
            {
                float dSatTemp = S;
                float dSatOK = 0.0f;
                float dSatNotOK = S;
                float dSatDiff = 1.0f;
                while (dSatDiff > 0.005f)
                {
                    HSPtoRGB_Int(H, dSatTemp, P, ref R, ref G, ref B, ref bOutOfGamut, nMode);
                    if (bOutOfGamut)
                    {
                        dSatNotOK = dSatTemp;
                        dSatTemp = dSatTemp - 0.5f * (dSatTemp - dSatOK);
                        if (dSatTemp < 0.01)
                        {
                            dSatTemp = 0.00f;
                            dSatDiff = 0.0f;
                        }
                    }
                    else
                    {
                        dSatOK = dSatTemp;
                        dSatDiff = 0.5f * (dSatNotOK - dSatOK);
                        if (dSatDiff > 0.005)
                            dSatTemp = dSatTemp + dSatDiff;
                    }
                }
                S = dSatTemp;
                HSPtoRGB_Int(H, S, P, ref R, ref G, ref B, ref bOutOfGamut, nMode);
            }
        }

        public void HSPtoRGB_Int(float H, float S, float P,
                ref float R, ref float G, ref float B, ref bool bOutOfGamut,
                int nMode)
        {
            //Hue Sat Perceived Brightness (Luminance)
            //H 0 to 360.0, S 0 to 1, P 0 to 1
            //http://alienryderflex.com/hsp.html

            //nMode = 1 Use PerceivedBrighness = Math.Sqrt(0.299 * r * r + 0.587 * g * g + 0.114 * b * b)
            //nMode = 2 Use PerceivedBrighness = 0.21 * r + 0.72 * g  + 0.07 * b

            //In Photoshop the Color blend mode preserves the luma of the bottom layer, 
            //  while adopting the hue and chroma of the top layer.

            float Pr = 0.299f;
            float Pg = 0.587f;
            float Pb = 0.114f;
            float part = 0.0f;
            float minOverMax = 1.0f - S;
            H = H / 360.0f;
            bOutOfGamut = false;

            if (minOverMax > 0.0)
            {
                //If 3>2>1
                //H = (2-1)/(3-1)
                //S = (3-1)/3

                if (H < 1.0 / 6.0)
                {
                    //  R>G>B
                    //H = (G-R)/(R-B)
                    //S = (R-B)/R
                    //minOverMax = 1-S = B/R
                    H = 6.0f * (H - 0.0f / 6.0f);
                    part = 1.0f + H * (1.0f / minOverMax - 1.0f); //=G/B
                    B = P / (float)Math.Sqrt(Pr / minOverMax / minOverMax + Pg * part * part + Pb); //Can rearrange to P2 = PbB2 + PgG2 + PrR2
                    R = (B) / minOverMax; //Since B/R = 1-S = minOverMax
                    G = (B) + H * ((R) - (B)); //Since (G-B)/(R-B) = H
                    //Check
                    //double dHue = (G - B) / (R - B)  - H;
                    //double dSat = (R - B) / R - S;
                    //double dP = Math.Sqrt(0.299 * R * R + 0.587 * G * G + 0.114 * B * B) -P;
                }
                else if (H < 2.0 / 6.0)
                {
                    //  G>R>B
                    //H = (R-B)/(G-B)
                    //S = (G-B)/G
                    H = 6.0f * (-H + 2.0f / 6.0f); part = 1.0f + H * (1.0f / minOverMax - 1.0f);
                    B = P / (float)Math.Sqrt(Pg / minOverMax / minOverMax + Pr * part * part + Pb);
                    G = (B) / minOverMax; R = (B) + H * ((G) - (B));
                }
                else if (H < 3.0 / 6.0)
                {
                    //  G>B>R
                    //H = (B-R)/(G-R)
                    //S = (G-R)/G
                    H = 6.0f * (H - 2.0f / 6.0f); part = 1.0f + H * (1.0f / minOverMax - 1.0f);
                    R = P / (float)Math.Sqrt(Pg / minOverMax / minOverMax + Pb * part * part + Pr);
                    G = (R) / minOverMax; B = (R) + H * ((G) - (R));
                }
                else if (H < 4.0 / 6.0)
                {
                    //  B>G>R
                    //H = (G-R)/(B-R)
                    //S = (B-R)/B
                    H = 6.0f * (-H + 4.0f / 6.0f); part = 1.0f + H * (1.0f / minOverMax - 1.0f);
                    R = P / (float)Math.Sqrt(Pb / minOverMax / minOverMax + Pg * part * part + Pr);
                    B = (R) / minOverMax; G = (R) + H * ((B) - (R));
                }
                else if (H < 5.0 / 6.0)
                {
                    //  B>R>G
                    //H = (R-G)/(B-G)
                    //S = (B-G)/B
                    H = 6.0f * (H - 4.0f / 6.0f); part = 1.0f + H * (1.0f / minOverMax - 1.0f);
                    G = P / (float)Math.Sqrt(Pb / minOverMax / minOverMax + Pr * part * part + Pg);
                    B = (G) / minOverMax; R = (G) + H * ((B) - (G));

                    //Adjust Sat (make it less), but keep Hue the same
                    if (B > 1.0d)
                    {
                        double dExcessP2 = Pb * (B * B - 1.0d);
                    }
                }
                else
                {
                    //  R>B>G
                    //H = (B-G)/(R-G)
                    //S = (R-G)/R
                    H = 6.0f * (-H + 6.0f / 6.0f); part = 1.0f + H * (1.0f / minOverMax - 1.0f);
                    G = P / (float)Math.Sqrt(Pr / minOverMax / minOverMax + Pb * part * part + Pg);
                    R = (G) / minOverMax; B = (G) + H * ((R) - (G));
                }
            }
            else
            {
                if (H < 1.0 / 6.0)
                {
                    //  R>G>B
                    H = 6.0f * (H - 0.0f / 6.0f);
                    R = (float)Math.Sqrt(P * P / (Pr + Pg * H * H)); G = (R) * H; B = 0.0f;
                }
                else if (H < 2.0 / 6.0)
                {
                    //  G>R>B
                    H = 6.0f * (-H + 2.0f / 6.0f);
                    G = (float)Math.Sqrt(P * P / (Pg + Pr * H * H)); R = (G) * H; B = 0.0f;
                }
                else if (H < 3.0 / 6.0)
                {
                    //  G>B>R
                    H = 6.0f * (H - 2.0f / 6.0f);
                    G = (float)Math.Sqrt(P * P / (Pg + Pb * H * H)); B = (G) * H; R = 0.0f;
                }
                else if (H < 4.0 / 6.0)
                {
                    //  B>G>R
                    H = 6.0f * (-H + 4.0f / 6.0f);
                    B = (float)Math.Sqrt(P * P / (Pb + Pg * H * H)); G = (B) * H; R = 0.0f;
                }
                else if (H < 5.0 / 6.0)
                {
                    //  B>R>G
                    H = 6.0f * (H - 4.0f / 6.0f);
                    B = (float)Math.Sqrt(P * P / (Pb + Pr * H * H)); R = (B) * H; G = 0.0f;
                }
                else
                {
                    //  R>B>G
                    H = 6.0f * (-H + 6.0f / 6.0f);
                    R = (float)Math.Sqrt(P * P / (Pr + Pb * H * H)); B = (R) * H; G = 0.0f;
                }
            }


            if (G < 0.0 || G > 1.0d || B < 0.0 || B > 1.0d || R < 0.0 || R > 1.0d)
                bOutOfGamut = true;
        }

        public void RGBtoHSB_PS(float red, float green, float blue, ref float h, ref float s, ref float v)
        {
            //http://stackoverflow.com/questions/4123998/algorithm-to-switch-between-rgb-and-hsb-color-values
            //RGB values are between 0 and 1

            float minValue = Math.Min(red, Math.Min(green, blue));
            float maxValue = Math.Max(red, Math.Max(green, blue));
            float delta = maxValue - minValue;

            h = 0.0f;
            s = 0.0f;
            v = maxValue;
            //https://en.wikipedia.org/wiki/HSL_and_HSV
            // Calculate the hue (in degrees of a circle, between 0 and 360)
            if (maxValue == red)
            {
                if (green >= blue)
                {
                    if (delta == 0)
                        h = 0;
                    else
                        h = 60 * (green - blue) / delta;
                }
                else if (green < blue)
                    h = 60 * (green - blue) / delta + 360;
            }
            else if (maxValue == green)
                h = 60 * (blue - red) / delta + 120;
            else if (maxValue == blue)
                h = 60 * (red - green) / delta + 240;

            //Calculate the saturation (between 0 and 1)
            if (maxValue == 0.0d)
                s = 0.0f;
            else
                s = 1.0f - (minValue / maxValue);
        }

        //Sharpening layers
        private bool mbLayersCalc = false;
        private Mat[] mLAYERS = new Mat[6];
        private Mat[] mLayerKernels = new Mat[6];
        private bool mbIgnoreLayersChanged = false;
        private bool mbIgnoreKernelSharpeningLayersChanged = false;

        private Mat KernelSharpeningLayers(int nKernelIndex)
        {
            Mat kernel = new Mat(3, 3, mImageTypeInternalChannel);
            if (nKernelIndex <= 0)
            {
                //Default Laplacian
                kernel = new Mat(3, 3, mImageTypeInternalChannel, new Scalar(-1.0f));
                kernel.Set<float>(1, 1, 8.0f); //Kernel sums to 0.0f
            }
            else if (nKernelIndex == 1)
            {
                //Lorentz times 2, alpha = 1
                kernel = new Mat(3, 3, mImageTypeInternalChannel, new Scalar(-1.0f)); //R2 = 1, fn = 2 x 1 / (1 + 1) = 1
                kernel.Set<float>(0, 0, -0.67f); //R2 = 2, so fn = 2 x 1 / (2 + 1) = 0.67
                kernel.Set<float>(0, 2, -0.67f);
                kernel.Set<float>(2, 0, -0.67f);
                kernel.Set<float>(2, 2, -0.67f);
                kernel.Set<float>(1, 1, 6.68f); //Kernel sums to 0.0f
            }
            else if (nKernelIndex == 2)
            {
                //Laplacian, no diagonals
                kernel = new Mat(3, 3, mImageTypeInternalChannel, new Scalar(0.0f));
                kernel.Set<float>(0, 1, -1f);
                kernel.Set<float>(1, 0, -1f);
                kernel.Set<float>(1, 2, -1f);
                kernel.Set<float>(2, 1, -1f);
                kernel.Set<float>(1, 1, 4.0f); //Kernel sums to 0.0f
            }
            else if (nKernelIndex == 3)
            {
                //Gaussian
                kernel = new Mat(3, 3, mImageTypeInternalChannel, new Scalar(-0.37f)); //R2 = 1, fn = 1/e
                kernel.Set<float>(0, 0, -0.135f); //Corners //R2 = 2, fn = 1/e2
                kernel.Set<float>(0, 2, -0.135f);
                kernel.Set<float>(2, 0, -0.135f);
                kernel.Set<float>(2, 2, -0.135f);
                kernel.Set<float>(1, 1, 2.02f); //Kernel sums to 0.0f
            }
            else if (nKernelIndex == 4)
            {
                //Gaussian 5x5
                kernel = new Mat(5, 5, mImageTypeInternalChannel, new Scalar(0.0f));
                int nR2 = 0;
                float fTot = 0.0f;
                float fCurrentVal = 0.0f;
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        nR2 = (y - 2) * (y - 2) + (x - 2) * (x - 2);
                        if (nR2 != 0)
                        {
                            fCurrentVal = (float)Math.Exp(-nR2);
                            fTot += fCurrentVal;
                            kernel.Set<float>(y, x, -fCurrentVal);
                        }
                    }
                }

                kernel.Set<float>(2, 2, fTot); //Kernel sums to 0.0f
            }
            else if (nKernelIndex == 5)
            {
                //Lorentz 5x5
                kernel = new Mat(5, 5, mImageTypeInternalChannel, new Scalar(0.0f));
                int nR2 = 0;
                float fTot = 0.0f;
                float fCurrentVal = 0.0f;
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        nR2 = (y - 2) * (y - 2) + (x - 2) * (x - 2);
                        if (nR2 != 0)
                        {
                            fCurrentVal = (float)(1.0f / (1 + nR2));
                            fTot += fCurrentVal;
                            kernel.Set<float>(y, x, -fCurrentVal);
                        }
                    }
                }

                kernel.Set<float>(2, 2, fTot); //Kernel sums to 0.0f
            }

            return kernel;
        }

        private void mSetUpSharpeningLayers()
        {
            float fNoiseControl = (float)udLayersNoiseControl.Value;
            float fScaleLayerSizeBy = (float)udLayersImageScale.Value;

            int nWidthToProcess = mnWidth;
            int nHeightToProcess = mnHeight;

            if (!mbLayersCalc && optSharpeningLayers.Checked && mbImageInLoaded)
            {
                this.Cursor = Cursors.WaitCursor;

                Mat imgToCalcLayersFor = new Mat(); //Just pointer, don't dispose
                if (optOriginal.Checked)
                    imgToCalcLayersFor = mHistory[0];
                else if (optPrevious.Checked)
                    imgToCalcLayersFor = mHistory[mHistoryCurrent];

                //If area to process defined
                if (mbROISet)
                {
                    imgToDeblur_Input = imgToCalcLayersFor.Clone();
                    Rect roi = new Rect(mnROIImageStartX, mnROIImageStartY, mnROIImageWidth, mnROIImageHeight);
                    imgToCalcLayersFor = new Mat(imgToCalcLayersFor, roi);
                    nWidthToProcess = mnROIImageWidth;
                    nHeightToProcess = mnROIImageHeight;
                }

                int nIterations = 6;
                int nKernelIndex = cboKernelSharpeningLayers.SelectedIndex;
                Mat kernel = KernelSharpeningLayers(nKernelIndex);

                mLAYERS = Layers(nWidthToProcess, nHeightToProcess, imgToCalcLayersFor, nIterations, kernel, 
                    fNoiseControl, nKernelIndex, fScaleLayerSizeBy, ref mLayerKernels);
                kernel.Dispose(); //For what it's worth
                mbLayersCalc = true;

                /*
                for (int i = 0; i < nIterations; i++)
                {
                    Mat mSave = new Mat(mnHeight, mnWidth, MatType.CV_16UC3);
                    mSave = (mLAYERS[i] + 0.5f) * 65536.0f;
                    mSave.ConvertTo(mSave, MatType.CV_16UC3);
                    Cv2.ImWrite(@"F:\temp\Pyramid\RGB" + i.ToString() + ".tif", mSave);
                }
                */

                this.Cursor = Cursors.Default;
            }
        }

        private float KernelScaleFactor(int nKernelMode)
        {
            //Scale some kernels higher (in intensity, not scale) to match appearance of other kernels
            float fScaleFactor = 1.0f;
            if (nKernelMode == 0)
                fScaleFactor = 1.0f;
            if (nKernelMode == 1)
                fScaleFactor = 1.35f;
            if (nKernelMode == 2)
                fScaleFactor = 3.0f;
            if (nKernelMode == 3)
                fScaleFactor = 5.0f;
            else if (nKernelMode == 4)
                fScaleFactor = 4.0f;
            else if (nKernelMode == 5)
                fScaleFactor = 0.7f;

            return fScaleFactor;
        }

        private Mat[] Layers (int nWidth, int nHeight, Mat imgInput, int nIter, 
                                Mat kernel, float fNoiseControl, int nKernelMode, float fScaleLayerSizeBy, ref Mat[] LayerKernels)
        {
            Mat[] imgIn = new Mat[mnChannels]; //Don't dispose, Mat passed in imgInput
            Cv2.Split(imgInput, out imgIn);
            Mat[][] mChannelLayers = new Mat[mnChannels][]; //Returned, equal to OutputLayers
            Mat[] OutputLayers = new Mat[nIter]; //Returned

            for (int nChannel = 0; nChannel < mnChannels; nChannel++)
            {
                mChannelLayers[nChannel] = LayerCalc(nIter, imgIn[nChannel], kernel, fNoiseControl, 
                    nKernelMode, fScaleLayerSizeBy, ref LayerKernels);
            }

            if (mnChannels != 1)
            {
                for (int layer = 0; layer < nIter; layer++)
                {
                    Mat[] mChannelComponents = new Mat[mnChannels];
                    for (int nChannel = 0; nChannel < mnChannels; nChannel++)
                    {
                        mChannelComponents[nChannel] = mChannelLayers[nChannel][layer];
                    }
                    OutputLayers[layer] = new Mat(nHeight, nWidth, mImageTypeInternal);
                    Cv2.Merge(mChannelComponents, OutputLayers[layer]);
                }
            }
            else
                OutputLayers = mChannelLayers[0];

            /*
            for (int nCh = 0; nCh < mChannelLayers.Length; nCh++)
            {
                for (int i = 0; i < mChannelLayers[nCh].Length; i++)
                {
                    mChannelLayers[nCh][i].Dispose();
                }
            }
            */
            GC.Collect();

            return OutputLayers;
        }

        private Mat[] LayerCalc(int nIter, Mat imgIn, Mat kernel, float fNoiseControl, int nKernelMode, 
            float fScaleLayerSizeBy, ref Mat[] LayerKernels)
        {
            //Use the same image, imgIn to convolve with a kernel which is double, same size, half size, 
            // quarter size, eight size, and one sixteenth size of original kernel
            //Except for the 0.5px layer, in this case double the input image in size, apply standard size
            // kernel, and resize down again
            //These become the six layers
            //1) Convolve kernel with Gaussian if  fNoiseControl > 0
            //2) Resize the kernel
            //3) Convolve the image, imgIn, with the resized kernel
            //See also LayerCalc_SameKernelSize

            Mat[] Layers = new Mat[nIter]; //Returned
            LayerKernels = new Mat[nIter];
            float fSigma = fNoiseControl;  //Same as doing a radius 1.0 gaussian blur in photoshop
            Mat kernelGauss = null;

            int nWidth = imgIn.Width;
            int nHeight = imgIn.Height;
            int nKernelWidth = kernel.Width;
            int nKernelHeight = kernel.Height;
            float fCurrentScale = 0.5f;
            float fKernelSum = 0.0f;

            //Scale some kernels higher (in intensity, not scale) to match appearance of other kernels
            float fScaleFactor = KernelScaleFactor(nKernelMode);

            Mat imgKernelResized = kernel.EmptyClone(); //Disposed
            Mat imgKernelBlurred = kernel.EmptyClone(); //Disposed
            float fLayerPowerScale = 1.0f; //Extra factor for each layer, 2px layer x 1.0, 1px x 2.0, 0.5px x 4 etc
            Mat imgDoubleSize = new Mat();
            Cv2.Resize(imgIn, imgDoubleSize, new Size(nWidth*2, nHeight*2), 0.0f, 0.0f, InterpolationFlags.Lanczos4);

            for (int i = 0; i < nIter; i++)
            {
                //If noise reduction selected, then Gaussian blur Convolved
                //Scale sigma according to fCurrentScale, ie less noise reduction required when
                // when have already resampled down
                if (fSigma != 0.0f)
                {
                    kernelGauss = GaussKernel(fSigma / (fCurrentScale * fScaleLayerSizeBy), 0.0005f);
                    imgKernelBlurred = kernelGauss.EmptyClone();
                    Cv2.Filter2D(kernelGauss, imgKernelBlurred, imgIn.Depth(), kernel, null, 0, BorderTypes.Reflect);
                }
                else
                    imgKernelBlurred = kernel.Clone();

                //Make imgKernelBlurred sum to 0.0, we want these to be DIFFERENCE layers, so sum to zero
                fKernelSum = (float)imgKernelBlurred.Sum().Val0;
                imgKernelBlurred = imgKernelBlurred - fKernelSum / imgKernelBlurred.Width / imgKernelBlurred.Height;

                Layers[i] = imgIn.EmptyClone();

                //Resize imgKernelBlurred to the required kernel size for this layer
                int nNewWidth = (int)Math.Floor(fCurrentScale * fScaleLayerSizeBy * imgKernelBlurred.Width);
                int nNewHeight = (int)Math.Floor(fCurrentScale * fScaleLayerSizeBy * imgKernelBlurred.Height);
                if (nNewHeight % 2 == 0)
                    nNewHeight = nNewHeight + 1;
                if (nNewWidth % 2 == 0)
                    nNewWidth = nNewWidth + 1;

                if (nNewWidth != imgKernelBlurred.Width || nNewHeight != imgKernelBlurred.Height)
                    Cv2.Resize(imgKernelBlurred, imgKernelResized, new Size(nNewWidth, nNewHeight), 0.0f,
                        0.0f, InterpolationFlags.Cubic);
                else
                    imgKernelResized = imgKernelBlurred;

                //Make imgKernelResized sum to 0.0, we want these to be DIFFERENCE layers, so sum to zero
                fKernelSum = (float)imgKernelResized.Sum().Val0;
                imgKernelResized = imgKernelResized - fKernelSum / imgKernelResized.Width / imgKernelResized.Height;

                //Convolve imgIn with imgKernelResized, store in Layers[i]
                if (i != 0)
                    Cv2.Filter2D(imgIn, Layers[i], imgIn.Depth(), imgKernelResized, null, 0, BorderTypes.Reflect);
                else
                {
                    //0.5 px scale - SPECIAL CASE
                    //Convolve double image with imgKernelBlurred, store in Layers[i]
                    Cv2.Filter2D(imgDoubleSize, Layers[0], imgIn.Depth(), imgKernelBlurred, null, 0, BorderTypes.Reflect);
                    //Resize Layers[0]
                    Cv2.Resize(Layers[0], Layers[0], new Size(nWidth, nHeight), 0.0f, 0.0f, InterpolationFlags.Lanczos4);
                }

                //Scale the layer intensity so that it matches LayerCalc_SameKernelSize 
                fLayerPowerScale = (float)Math.Pow(12.0d, 1.4 - i - fScaleLayerSizeBy + 1.0) ;
                Layers[i] = Layers[i] * fLayerPowerScale * fScaleFactor;

                LayerKernels[i] = imgKernelResized.Clone() * fLayerPowerScale * fScaleFactor;

                fCurrentScale = fCurrentScale * 2.0f;
            }

            imgDoubleSize.Dispose();
            imgKernelResized.Dispose();
            imgKernelBlurred.Dispose();
            GC.Collect();

            return Layers;
        }


        private Mat[] LayerCalc_SameKernelSize(int nIter, Mat imgIn, Mat kernel, float fNoiseControl, int nKernelMode, float fScaleLayerSizeBy)
        {
            //Use the same kernel to convolve with an image size which is double, same size, half size, 
            // quarter size, eight size, and one sixteenth size of original
            //1) Change image size
            //2) Convolve with kernel
            //3) Convolve with Gaussian if  fNoiseControl > 0
            //See also LayerCalc

            Mat[] Layers = new Mat[nIter]; //Returned
            Mat Convolved = imgIn.EmptyClone(); //Disposed
            float fSigma = fNoiseControl;  //Same as doing a radius 1.0 gaussian blur in photoshop
            Mat kernelGauss = null;
            //Start with larger than input image, ie at 0.5px scale
            int nCurrentWidth = imgIn.Width  * 2;
            int nCurrentHeight = imgIn.Height * 2;
            if (fScaleLayerSizeBy != 1.0f)
            {
                nCurrentWidth = (int)Math.Round(nCurrentWidth / fScaleLayerSizeBy, 0);
                nCurrentHeight = (int)Math.Round(nCurrentHeight / fScaleLayerSizeBy, 0);
            }

            //Scale some kernels higher (in intensity, not scale) to match appearance of other kernels
            float fScaleFactor = KernelScaleFactor(nKernelMode);

            float fLayerPowerScale = 1.0f; //Extra factor for each layer, 2px layer x 1.0, 1px x 2.0, 0.5px x 4 etc

            Mat imgResized = imgIn.EmptyClone(); //Disposed
            Cv2.Resize(imgIn, imgResized, new Size(nCurrentWidth, nCurrentHeight), 0.0f, 0.0f, InterpolationFlags.Linear);

            for (int i = 0; i < nIter; i++)
            {
                //Convolve imgIn with kernel into Convolved
                Cv2.Filter2D(imgResized, Convolved, imgIn.Depth(), kernel, null, 0, BorderTypes.Reflect);

                //If noise reduction selected, then Gaussian blur Convolved
                //Scale sigma according to the current image size, ie less noise reduction required when
                // when have already resampled down
                if (fSigma != 0.0f)
                {
                    kernelGauss = GaussKernel(fSigma * (float)nCurrentWidth / (float)imgIn.Width  , 0.001f);
                    Cv2.Filter2D(Convolved, Convolved, imgIn.Depth(), kernelGauss, null, 0, BorderTypes.Reflect);
                }

                Layers[i] = imgIn.EmptyClone();
                //Resize Convolved to the actual image size, normally an increase in size
                Cv2.Resize(Convolved, Layers[i], new Size(imgIn.Width, imgIn.Height), 0.0f, 0.0f, InterpolationFlags.Lanczos4);

                fLayerPowerScale = (float)Math.Pow(2.0d, 2 - i);
                Layers[i] = Layers[i] * fScaleFactor * fLayerPowerScale;

                //Resize imgResized at half current size for next time in loop
                nCurrentWidth = nCurrentWidth / 2;
                nCurrentHeight = nCurrentHeight / 2;
                Cv2.Resize(imgIn, imgResized, new Size(nCurrentWidth, nCurrentHeight), 0.0f, 0.0f, InterpolationFlags.Linear);

                GC.Collect();
            }

            Convolved.Dispose();
            imgResized.Dispose();
            GC.Collect();

            return Layers;
        }

        private void CombineLayers(float fScaleLayers)
        {
            if (!ImageLoad())
            {
                return;
            }

            try
            {
                if (!mbLayersCalc)
                    mSetUpSharpeningLayers();

                float fAllLayerAdjust = 1.0f;
                float[] fLayerWgts = new float[6];
                if (chkLayers0.Checked)
                    fLayerWgts[0] = trkLayers0.Value;
                if (chkLayers1.Checked)
                    fLayerWgts[1] = trkLayers1.Value;
                if (chkLayers2.Checked)
                    fLayerWgts[2] = trkLayers2.Value;
                if (chkLayers3.Checked)
                    fLayerWgts[3] = trkLayers3.Value;
                if (chkLayers4.Checked)
                    fLayerWgts[4] = trkLayers4.Value;
                if (chkLayers5.Checked)
                    fLayerWgts[5] = trkLayers5.Value;

                Mat imgBase = new Mat();
                if (optOriginal.Checked)
                    imgBase = mHistory[0];
                else if (optPrevious.Checked)
                    imgBase = mHistory[mHistoryCurrent];

                if (mbROISet)
                    mREPAIRED = new Mat(imgBase, new Rect(mnROIImageStartX, mnROIImageStartY, mnROIImageWidth, mnROIImageHeight));
                else
                    mREPAIRED = imgBase.Clone();

                float fTotalLayerWgts = 0.0f;
                for (int i = 0; i < fLayerWgts.Length; i++)
                {
                    fTotalLayerWgts = fTotalLayerWgts + fLayerWgts[i];
                }
                float fMultiplyBy = 1.0f;

                if (fTotalLayerWgts != 0.0f)
                {
                    for (int i = 0; i < fLayerWgts.Length; i++)
                    {
                        if (fLayerWgts[i] != 0.0f)
                        {
                            fMultiplyBy = fLayerWgts[i] * fScaleLayers / fAllLayerAdjust / fTotalLayerWgts;
                            mREPAIRED = mREPAIRED + mLAYERS[i] * fMultiplyBy;
                        }
                    }
                }

                mREPAIRED_Unstretched = mREPAIRED.Clone();

                //Calculate the repairing psf that is being applied to the input image, ie, the psf used to deconvolve
                //This is the sum of the kernels in mLayerKernels, weighted by the currently selected weights defined in 
                // the user interface
                int nCurKernelWidth = 0;
                int nCurKernelHeight = 0;
                Mat psfCombined = new Mat( mLayerKernels[mLayerKernels.Length - 1].Height,
                    mLayerKernels[mLayerKernels.Length - 1].Width, mLayerKernels[0].Type(), new Scalar(0.0f)); //The biggest kernel
                //Central pixel in psfCombined is the original image, so set to 1.0
                psfCombined.Set<float>(psfCombined.Height / 2, psfCombined.Width / 2, 1.0f);
                Mat psfFullsizeKernel = new Mat();
                if (fTotalLayerWgts != 0.0f)
                {
                    for (int i = 0; i < fLayerWgts.Length; i++)
                    {
                        if (fLayerWgts[i] != 0.0f)
                        {
                            fMultiplyBy = fLayerWgts[i] * fScaleLayers / fAllLayerAdjust / fTotalLayerWgts;
                            psfFullsizeKernel = new Mat(psfCombined.Height, psfCombined.Width, psfCombined.Type(), new Scalar(0.0f));

                            nCurKernelWidth = mLayerKernels[i].Width;
                            nCurKernelHeight = mLayerKernels[i].Height;

                            //Copy small kernel to psfFullsizeKernel
                            Rect roi = new Rect(psfCombined.Width / 2- nCurKernelWidth/2,
                                psfCombined.Height / 2 - nCurKernelHeight / 2, nCurKernelWidth, nCurKernelHeight);
                            mLayerKernels[i].CopyTo(new Mat(psfFullsizeKernel, roi));

                            //Stack this kernel onto the combined kernel
                            psfCombined = psfCombined + psfFullsizeKernel * fMultiplyBy;
                        }
                    }
                }
                //Store the kernel used to repair
                mPsfREPAIRED = psfCombined.Clone();
                psfFullsizeKernel.Dispose();

                //Calculate FT of the repairing PSF and display in picFilterFT
                Mat real = new Mat(); //Disposed
                Mat imag = new Mat(); //Disposed
                DFT(psfCombined, null, ref real, ref imag, DftFlags.None);
                Mat filterFT = real.Mul(real) + imag.Mul(imag);
                Cv2.Pow(filterFT, 0.5d, filterFT);
                if (optFilterFourierTransform.Checked)
                    mDisplayFFTInPic(filterFT, picFilterFT, true, 0.0d, (double)udMaxPlotFT.Value);
                //Store this FT
                mFTDisplayREPAIRED = filterFT;
                //Calculate the PSF that would have been used to convolve, ie InverseFT(1/filterFT)
                mPsfInvREPAIRED = PsfFromFT(real, imag);
                mPsfInvREPAIRED = rearrangeQuadrants(mPsfInvREPAIRED);
                mFWHMREPAIRED = 0.0f;
                //And display
                if (fTotalLayerWgts == 0.0f)
                {
                    picPSFProfile.Image = null;
                    picPSF.Image = null;
                }
                else
                {
                    Mat psfDisplay = new Mat();
                    if (chkInvPSF.Checked)
                        psfDisplay = mPsfInvREPAIRED;
                    else
                        psfDisplay = mPsfREPAIRED;

                    PlotPSFProfile(psfDisplay, picPSFProfile, (int)udPSFPlotWidth.Value, 0.0f);
                    Mat psfCropped = PSFCroppedForDisplay(psfDisplay, (int)(udPSFPlotWidth.Value), 65000.0f);
                    MatToPictureBox(psfCropped, picPSF, false, new System.Drawing.Point());
                    psfCropped.Dispose();
                }
                real.Dispose();
                imag.Dispose();
                psfCombined.Dispose();

                //Description of repair
                msRepairedDesription = "SHARPENING LAYERS ";
                if (chkLayers0.Checked)
                    msRepairedDesription += "0.5px=" + trkLayers0.Value.ToString() + " ";
                if (chkLayers1.Checked)
                    msRepairedDesription += "1px=" + trkLayers1.Value.ToString() + " ";
                if (chkLayers2.Checked)
                    msRepairedDesription += "2px=" + trkLayers2.Value.ToString() + " ";
                if (chkLayers3.Checked)
                    msRepairedDesription += "4px=" + trkLayers3.Value.ToString() + " ";
                if (chkLayers4.Checked)
                    msRepairedDesription += "8px=" + trkLayers4.Value.ToString() + " ";
                if (chkLayers5.Checked)
                    msRepairedDesription += "16px=" + trkLayers5.Value.ToString() + " ";

                msRepairedDesription += " Mode-" +
                    cboKernelSharpeningLayers.SelectedItem.ToString().Substring(0, 9).Replace("(", "").Replace(")", "") 
                    + " Noise-" + udLayersNoiseControl.Value.ToString()
                    + " Contrast-" + udLayersScale.Value.ToString();
                if (udLayersImageScale.Value != 1.0m)
                {
                    msRepairedDesription += " Layer scale x " + udLayersImageScale.Value.ToString();
                }

                if (chkAutostretch.Checked)
                    mREPAIRED = StretchImagePercentiles(mREPAIRED, mnPercentilesStretchMin, mnPercentilesStretchMax);

                //Display the repaired image
                if (mbROISet)
                {
                    //Copy repaired mREPAIRED over the top of imgToDeblur_Input
                    Mat mSmall = mREPAIRED.Clone();
                    mREPAIRED = imgToDeblur_Input;
                    Rect roi = new Rect(mnROIImageStartX, mnROIImageStartY, mnROIImageWidth, mnROIImageHeight);
                    mSmall.CopyTo(new Mat(mREPAIRED, roi));
                    mSmall.Dispose();
                }
                MatToPictureBox_Zoomed(mREPAIRED, picOut, msRepairedDesription, true, false, false);

                if (cboCurrent.Items[cboCurrent.Items.Count - 1].ToString() != "REPAIRED")
                    cboCurrent.Items.Add("REPAIRED");

                if (cboCurrent.SelectedIndex != cboCurrent.Items.Count - 1)
                {
                    mbIgnoreCurrentDisplayCboChange = true;
                    cboCurrent.SelectedIndex = cboCurrent.Items.Count - 1;
                    mbIgnoreCurrentDisplayCboChange = false;
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                txtDebug.Visible = true;
                lblDebugError.Visible = true;
                txtDebug.Text = "ERROR: Combine Layers " + ex.ToString();
                txtDebug.SelectAll();
                txtDebug.Focus();

                this.Cursor = Cursors.Default;
            }
        }

        //Apply a convolution
        private void btnConvolve_Click(object sender, EventArgs e)
        {
            mbDeblur = false;
            btnDeblur_Click(null, null);
        }

        //WIENER Filter
        private Mat applyWienerFilter(Mat inputImg, Mat psf, double dNoiseToSignalRatio, ref Mat WnrFilter_Real, ref Mat WnrFilter_Imag)
        {
            //Image input z (Greyscale value at x,y) = SUM ( a * Sin (hx + ky) )
            //Each pixel in the Fourier transform has a coordinate (h,k) representing the contribution 
            // of the sine wave with x-frequency h, and y-frequency k in the Fourier transform. 
            //The centre point represents the (0,0) wave – a flat plane with no ripples – 
            //and its intensity (its brightness in colour in the grey scale) is the average value of the pixels in the image

            //Rearrange the quadrants of the input PSF
            Mat psf_shifted = rearrangeQuadrants(psf); //Disposed
            Mat PSF_FTReal = new Mat(); //Disposed
            Mat PSF_FTImag = new Mat(); //Disposed
            //Fourier transform of shifted input PSF
            DFT(psf_shifted, null, ref PSF_FTReal, ref PSF_FTImag, DftFlags.None);
            psf_shifted.Dispose();

            //Denominator of Wiener filter = ABS(PSF Fourier transform)^2 + NSR
            Mat denom = psf.EmptyClone(); //Disposed
            denom = PSF_FTReal.Mul(PSF_FTReal) + PSF_FTImag.Mul(PSF_FTImag) 
                    + dNoiseToSignalRatio; //Real only

            //Wiener Filter = Complex conjugate of (PSF Fourier Transform) / Denominator
            WnrFilter_Real = psf.EmptyClone();
            Cv2.Divide(PSF_FTReal, denom, WnrFilter_Real);
            WnrFilter_Imag = psf.EmptyClone(); //Set to inputPlanes[1]
            Cv2.Divide(-PSF_FTImag, denom, WnrFilter_Imag); //The conjugate of, so minus

            PSF_FTReal.Dispose();
            PSF_FTImag.Dispose();
            denom.Dispose();
            GC.Collect();

            //Combine WnrFilter_Real and WnrFilter_Imag into a complex Mat, called WienerFilter
            // The complex Mat is needed when calling Cv2.MulSpectrums
            Mat WienerFilter = psf.EmptyClone();
            Mat[] inputPlanes = new Mat[2]; //Disposed
            inputPlanes[0] = WnrFilter_Real; //Returned, not disposed
            inputPlanes[1] = WnrFilter_Imag;  //Disposed
            Cv2.Merge(inputPlanes, WienerFilter);
            GC.Collect();

            if (mbDebug)
            {
                StringBuilder sbDebug = new StringBuilder();
                Mat Mag = inputImg.EmptyClone();
                //Magnitude of WnrFilter
                Cv2.Magnitude(WnrFilter_Real, WnrFilter_Imag, Mag);
                for (int x = 0; x <= mnWidth / 2; x++)
                {
                    sbDebug.Append(x.ToString() + "\t" + WnrFilter_Real.At<float>(0, x).ToString()
                        + "\t" + WnrFilter_Imag.At<float>(0, x).ToString() + "\t" +
                        Mag.At<float>(0, x).ToString() + "\r\n");
                }
                txtDebug.Text = sbDebug.ToString();
                //What PSF is WienerFilter the same as ?
                Mat PSFWiener = WienerFilter.EmptyClone();
                Cv2.Idft(WienerFilter, PSFWiener, DftFlags.Scale);
                Mat[] planesPSF = new Mat[2];
                planesPSF = Cv2.Split(PSFWiener);
                planesPSF[0] = rearrangeQuadrants(planesPSF[0]);
                planesPSF[1] = rearrangeQuadrants(planesPSF[1]);
                Cv2.Magnitude(planesPSF[0], planesPSF[1], Mag);
                sbDebug = new StringBuilder();
                for (int x = mnWidth / 2; x >= 0; x--)
                {
                    sbDebug.Append(x.ToString() + "\t" + planesPSF[0].At<float>(mnHeight/2, x).ToString()
                        + "\t" + planesPSF[1].At<float>(mnHeight / 2, x).ToString() + "\t" +
                        Mag.At<float>(mnHeight / 2, x).ToString() + "\r\n");
                }
                txtDebug.Text = sbDebug.ToString();

                Mag.Dispose();
            }

            //APPLY the Wiener filter
            //Fourier transform on input image
            Mat InputImage_FTReal = new Mat(); //Disposed
            Mat InputImage_FTImag = new Mat(); //Disposed
            Mat InputImage_FT = DFT(inputImg, null, ref InputImage_FTReal, 
                ref InputImage_FTImag, DftFlags.Scale); //Disposed
                //DftFlags.Scale = Scales the result: divide it by the number of array elements
            InputImage_FTReal.Dispose();
            InputImage_FTImag.Dispose();

            //Multiply FT spectrums: input image (FT) x Wiener filter (FT)
            Mat multipliedResult = WienerFilter.EmptyClone(); //Disposed
            Cv2.MulSpectrums(InputImage_FT, WienerFilter, multipliedResult, DftFlags.None);
            InputImage_FT.Dispose();

            //Inverse FT on product
            Mat inversedFT = WienerFilter.EmptyClone(); //Disposed
            Cv2.Idft(multipliedResult, inversedFT);
            multipliedResult.Dispose();

            Mat[] planes = new Mat[2]; //Disposed planes[1], return planes[0]
            Cv2.Split(inversedFT, out planes);

            inversedFT.Dispose();
            planes[1].Dispose();
            GC.Collect();

            return planes[0]; //Real component
        }

        //INVERSE Filter (Regularised with Lapacian operator)
        private Mat applyInverseLaplacianRegularisedFilter(Mat inputImg, Mat psf, double dNoiseToSignalRatio, 
            ref Mat Filter_Real, ref Mat Filter_Imag)
        {
            //DeconvolutionLab2 RIF method "Regularized inverse filtering"
            //D. Sage et al. / Methods 115 (2017) 28–41: 3.4

            //Rearrange the quadrants of the input PSF
            Mat psf_shifted = rearrangeQuadrants(psf); //Disposed
            Mat PSF_FTReal = new Mat(); //Disposed
            Mat PSF_FTImag = new Mat(); //Disposed
            //Fourier transform of shifted input PSF
            DFT(psf_shifted, null, ref PSF_FTReal, ref PSF_FTImag, DftFlags.None);
            psf_shifted.Dispose();

            //Fourier transform on input image
            Mat InputImage_FTReal = new Mat(); //Disposed
            Mat InputImage_FTImag = new Mat(); //Disposed
            Mat InputImage_FT = DFT(inputImg, null, ref InputImage_FTReal,
                ref InputImage_FTImag, DftFlags.Scale); //Disposed
                                                        //DftFlags.Scale = Scales the result: divide it by the number of array elements
            InputImage_FTReal.Dispose();
            InputImage_FTImag.Dispose();

            //Discretization of a differential operator
            Mat lapOperatorSquared = laplacianOperatorSquared(inputImg, inputImg.Width, inputImg.Height);

            //Denominator of filter = ABS(PSF Fourier transform)^2 + ABS(Laplacian Fourier transform)^2 x NSR
            Mat denom = psf.EmptyClone(); //Disposed
            denom = PSF_FTReal.Mul(PSF_FTReal) + PSF_FTImag.Mul(PSF_FTImag) 
                    + lapOperatorSquared * dNoiseToSignalRatio; //Real only
            lapOperatorSquared.Dispose();

            //Numerator of filter = Complex conjugate of (PSF Fourier Transform)
            //Filter to apply = Numerator / Denominator
            Filter_Real = psf.EmptyClone();
            Cv2.Divide(PSF_FTReal, denom, Filter_Real);
            Filter_Imag = psf.EmptyClone(); //Set to inputPlanes[1] which is disposed
            Cv2.Divide(-PSF_FTImag, denom, Filter_Imag); //The conjugate of, so minus

            PSF_FTReal.Dispose();
            PSF_FTImag.Dispose();
            denom.Dispose();
            GC.Collect();

            //Combine Filter_Real and Filter_Imag into a complex Mat, called Filter
            // The complex Mat is needed when calling Cv2.MulSpectrums
            Mat Filter = psf.EmptyClone();
            Mat[] inputPlanes = new Mat[2]; //Disposed
            inputPlanes[0] = Filter_Real; //Returned, not disposed
            inputPlanes[1] = Filter_Imag;   //Returned, not disposed
            Cv2.Merge(inputPlanes, Filter);
            GC.Collect();

            //APPLY the filter
            //Multiply FT spectrums: input image (FT) x filter (FT)
            Mat multipliedResult = Filter.EmptyClone(); //Disposed
            Cv2.MulSpectrums(InputImage_FT, Filter, multipliedResult, DftFlags.None);
            InputImage_FT.Dispose();

            //Inverse FT on product
            Mat inversedFT = Filter.EmptyClone(); //Disposed
            Cv2.Idft(multipliedResult, inversedFT);
            multipliedResult.Dispose();

            Mat[] planes = new Mat[2]; //Disposed planes[1], return planes[0]
            Cv2.Split(inversedFT, out planes);

            inversedFT.Dispose();
            planes[1].Dispose();
            GC.Collect();

            return planes[0]; //Real component
        }

        //TIKHONOV Filter
        private Mat applyTikhonovFilter(Mat inputImg, Mat PSF, double Y, ref Mat tikhonovValueReal, ref Mat tikhonovValueImag)
        {
            //https://github.com/pratscy3/Inverse-Filtering
            //Also called Tikhonov filtration, or Tikhonov regularization
            //http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.369.2500&rep=rep1&type=pdf

            //Fourier transform on input image
            Mat inputImg_FTReal = new Mat(); //Disposed
            Mat inputImg_FTImag = new Mat(); //Disposed
            Mat inputImgFT = DFT(inputImg, null, ref inputImg_FTReal, ref inputImg_FTImag, DftFlags.Scale); //Disposed
            inputImg_FTReal.Dispose();
            inputImg_FTImag.Dispose();

            //The 2-D Laplacian of Gaussian (LoG) function matrix is
            // 0 -1  0
            //-1  4 -1
            // 0 -1  0
            Rect rSize = new Rect(0, 0, inputImg.Cols, inputImg.Rows);
            Mat laplacian = new Mat(rSize.Size, mImageTypeInternalChannel, new Scalar(0)); //Disposed
            laplacian.Set<float>(0, 1, -1.0f);
            laplacian.Set<float>(1, 0, -1.0f);
            laplacian.Set<float>(0, 0, 4.0f);
            laplacian.Set<float>(inputImg.Rows - 1, 0, -1.0f);
            laplacian.Set<float>(0, inputImg.Cols - 1, -1.0f);

            //Fourier transform on 2-D Laplacian
            Mat laplacianFFT_Real = new Mat(); //Disposed
            Mat laplacianFFT_Imag = new Mat(); //Disposed
            DFT(laplacian, null, ref laplacianFFT_Real, ref laplacianFFT_Imag, DftFlags.None);
            laplacian.Dispose();

            //Fourier transform on shifted PSF
            Mat PSF_shifted = rearrangeQuadrants(PSF); //Disposed
            Mat PSF_FTReal = new Mat(); //Disposed
            Mat PSF_FTImag = new Mat(); //Disposed
            Mat PSF_shifted_FT = DFT(PSF_shifted, null, ref PSF_FTReal, ref PSF_FTImag, DftFlags.None); //Disposed
            PSF_shifted.Dispose();

            //Denominator of Tikhonov filter = ABS(PSF Fourier transform)^2 + Y * ABS(Laplacian Fourier transform)^2
            Mat denom = inputImg.EmptyClone();
            denom = PSF_FTReal.Mul(PSF_FTReal) + PSF_FTImag.Mul(PSF_FTImag) + 
                Y * 389.63f / 64.0f * (laplacianFFT_Real.Mul(laplacianFFT_Real) + laplacianFFT_Imag.Mul(laplacianFFT_Imag));
            //* 389.63f / 64.0f So that maximum of (laplacianFFT_Real * laplacianFFT_Real + laplacianFFT_Imag * laplacianFFT_Imag)
            //  is the same as in Regularised inverse filter

            laplacianFFT_Real.Dispose();
            laplacianFFT_Imag.Dispose();

            //Numerator of filter = Complex conjugate of (PSF Fourier Transform)
            //Filter to apply = Numerator / Denominator
            tikhonovValueReal = inputImg.EmptyClone(); //Mat passed in by ref
            tikhonovValueReal = PSF_FTReal / denom;
            tikhonovValueImag = inputImg.EmptyClone(); //Disposed
            tikhonovValueImag = PSF_FTImag / denom;
            PSF_FTReal.Dispose();
            PSF_FTImag.Dispose();
            denom.Dispose();

            //Combine the real and imaginary components to make the complex Tikhonov filter, to use with Cv2.MulSpectrums
            Mat multipliedResult = PSF_shifted_FT.EmptyClone(); //Disposed
            Mat tikhonov = PSF_shifted_FT.EmptyClone(); //Disposed
            Mat[] tikhonov_Planes = new Mat[2]; //Returned, not disposed
            tikhonov_Planes[0] = tikhonovValueReal; //Mat passed in by ref
            tikhonov_Planes[1] = tikhonovValueImag;
            Cv2.Merge(tikhonov_Planes, tikhonov);

            //Apply the Tikhonov filter
            Cv2.MulSpectrums(inputImgFT, tikhonov, multipliedResult, DftFlags.None);
            inputImgFT.Dispose();
            tikhonov.Dispose();

            //Inverse FT on product
            Mat inversedResult = PSF_shifted_FT.EmptyClone(); //Disposed
            Cv2.Idft(multipliedResult, inversedResult);
            PSF_shifted_FT.Dispose();
            multipliedResult.Dispose();

            Mat[] outputPlanes = new Mat[2]; //One disposed, one returned
            Cv2.Split(inversedResult, out outputPlanes);
            inversedResult.Dispose();
            outputPlanes[1].Dispose();

            GC.Collect();

            return outputPlanes[0]; //Real component
        }

        //LANDWEBER deconvolution
        private Mat Landweber(Mat imgIn, Mat PSF, int nIterationCount,
                int nBlurQuality, int nChannel, bool bRegularize)
        {
            int nWidth = imgIn.Width;
            int nHeight = imgIn.Height;

            SetUpIterations(imgIn, nChannel, nIterationCount);
            //Convert input image to float in range 0 to 1
            Mat observed = imgIn;
            Mat repaired = observed.Clone();

            //As in SmartDeblur deconvolutionByTotalVariationPrior
            double[] outputImageMatrix = new double[mnWidth * mnHeight];
            double dBlurSmooth = (double)nBlurQuality; //Equivalent to about NSR of 1000, the slider in SmartDeblur 0->100

            double epsilon = 0.004;
            double epsilonPow2 = epsilon * epsilon;
            double lambda = Math.Pow(1.07, dBlurSmooth) / 100000.0; //Range 0.00001 to 0.0087
            double tau = 1.9 / (1 + lambda * 8 / epsilon); //Range 1.9 to 0.1

            //Truncate PSF in areas where it is zero
            Mat PSF_kernel = cropMat(PSF);

            //PSF convolved with itself
            Mat PSF_Squared = PSF.EmptyClone();
            Cv2.Filter2D(PSF, PSF_Squared, -1, PSF_kernel, null, 0, BorderTypes.Reflect);
            //Truncate PSF_Squared
            PSF_Squared = cropMat(PSF_Squared);
            PSF_Squared = PSF_Squared / (float)PSF_Squared.Sum()[0];

            //Convolve observed with PSF
            Mat Observed_x_PSF = observed.EmptyClone();
            Cv2.Filter2D(observed, Observed_x_PSF, -1, PSF_kernel, null, 0, BorderTypes.Reflect);

            double[] repaired_Array = new double[mnSize];
            double[] divergence_Array = new double[mnSize];
            Mat divergence = observed.EmptyClone(); //Disposed after loop
            string sChName = "";

            Mat Repaired_x_PSF2 = observed.EmptyClone();

            for (int iteration = 0; iteration <= nIterationCount; iteration++)
            {
                if (mbProcessingCancelled)
                {
                    mbIgnoreIterationsChanged = false;
                    break;
                }

                if (iteration % mnIterationUIFactor == 0)
                {
                    //Save iteration repaired Into mdRestoredMatEst
                    mDeconvolvedMatIterations[nChannel][iteration / mnIterationUIFactor] = repaired.Clone();
                    if (mnChannels == 1)
                        sChName = "";
                    else
                    {
                        sChName = "Ch:";
                        if (nChannel == 0)
                            sChName += "B";
                        if (nChannel == 1)
                            sChName += "G";
                        if (nChannel == 2)
                            sChName += "R";
                    }
                    lblIterationInfo.Text = sChName + " " + iteration.ToString();
                    Application.DoEvents();
                }

                //Convolve repaired with PSF_Squared
                Cv2.Filter2D(repaired, Repaired_x_PSF2, -1, PSF_Squared, null, 0, BorderTypes.Reflect);

                //Calculate divergence
                if (bRegularize)
                    divergence = calcDivergence(nWidth, nHeight, (float)epsilonPow2, repaired);

                //Calculation (with regularisation term)
                //repaired(i+1) = repaired(i) - tau *
                //         (repaired(i)**(psf**psf) - o**p + lambda * Divergence(repaired(i)));
                if (bRegularize)
                    repaired = repaired - tau * (Repaired_x_PSF2 - Observed_x_PSF + lambda * divergence);
                else
                    repaired = repaired - tau * (Repaired_x_PSF2 - Observed_x_PSF);
            }

            if (divergence != null)
                divergence.Dispose();
            if (Repaired_x_PSF2 != null)
                Repaired_x_PSF2.Dispose();
            if (PSF_Squared != null)
                PSF_Squared.Dispose();
            if (Observed_x_PSF != null)
                Observed_x_PSF.Dispose();
            GC.Collect();

            //Final output into Mat (for this channel)
            mbIgnoreIterationsChanged = false;

            return repaired;
        }

        //LANDWEBER deconvolution FT, Forcing colvolutions to be calculated using FTs, faster
        private Mat Landweber_FourierTransform(Mat imgIn, Mat PSF, int nIterationCount, 
            int nBlurQuality, int nChannel, bool bRegularize)
        {
            int nWidth = imgIn.Width;
            int nHeight = imgIn.Height;

            SetUpIterations(imgIn, nChannel, nIterationCount);
            //Convert input image to float in range 0 to 1
            Mat observed = imgIn;
            Mat repaired = observed.Clone();

            //Deconvolution by Total Variation Regularization
            //https://www.numerical-tours.com/matlab/inverse_2_deconvolution_variational/#46
            //Also in SmartDeblur deconvolutionByTotalVariationPrior

            double[] outputImageMatrix = new double[mnWidth * mnHeight];
            //nBlurQuality passed in as zero
            double dBlurSmooth = (double)nBlurQuality; //Equivalent to about NSR of 1000, the slider in SmartDeblur 0->100

            double epsilon = 0.004;
            double epsilonPow2 = epsilon * epsilon;
            double lambda = Math.Pow(1.07, dBlurSmooth) / 100000.0; //Range 0.00001 to 0.0087
            //lambda will be 1e-5 with blur quality 0
            double tau = 1.9 / (1 + lambda * 8 / epsilon); //Range 1.9 to 0.1, will be 1.863 with blur quality 0

            //Rearrange the quadrants of the input PSF
            Mat PSF_shifted = rearrangeQuadrants(PSF); //Disposed
            //Transform PSF to PSF_FFT_Squared
            Mat FTReal = new Mat(); //Disposed after loop
            Mat FTImag = new Mat(); //Disposed after loop
            Mat PSF_FFT = DFT(PSF_shifted, null, ref FTReal, ref FTImag, DftFlags.None); //Disposed //Divide by no of elements 
            Mat PSF_FFT_Squared = PSF_FFT.EmptyClone(); //Disposed after loop
            Cv2.MulSpectrums(PSF_FFT, PSF_FFT, PSF_FFT_Squared, DftFlags.None);
            PSF_shifted.Dispose();

            //Convolve observed with PSF
            Mat Observed_FFT = DFT(observed, null, ref FTReal, ref FTImag, DftFlags.Scale); //Disposed //Divide by no of elements 
            Mat Observed_FFTxPSF_FFT = Observed_FFT.EmptyClone(); //Disposed
            Cv2.MulSpectrums(Observed_FFT, PSF_FFT, Observed_FFTxPSF_FFT, DftFlags.None);
            Mat inversedResult = Observed_FFT.EmptyClone(); //Disposed after loop
            Cv2.Idft(Observed_FFTxPSF_FFT, inversedResult);
            Observed_FFTxPSF_FFT.Dispose();
            PSF_FFT.Dispose();

            Mat[] outputPlanes = new Mat[2]; //Disposed after loop
            Cv2.Split(inversedResult, out outputPlanes);
            Mat Observed_x_PSF = outputPlanes[0]; //No need to dispose, just a pointer

            Mat Repaired_FFT = null; //Disposed after loop
            Mat Repaired_x_PSF_x_PSF = null; //Disposed after loop
            Mat Repaired_FFTxPSF_FFT_Squared = Observed_FFT.EmptyClone(); //Disposed after loop
            Observed_FFT.Dispose();

            double[] repaired_Array = new double[mnSize];
            double[] divergence_Array = new double[mnSize];
            Mat divergence = observed.EmptyClone(); //Disposed after loop
            string sChName = "";

            for (int iteration = 0; iteration <= nIterationCount; iteration++)
            {
                if (mbProcessingCancelled)
                {
                    mbIgnoreIterationsChanged = false;
                    break;
                }

                if (iteration % mnIterationUIFactor == 0)
                {
                    //Save iteration repaired Into mdRestoredMatEst
                    mDeconvolvedMatIterations[nChannel][iteration / mnIterationUIFactor] = repaired.Clone();
                    if (mnChannels == 1)
                        sChName = "";
                    else
                    {
                        sChName = "Ch:";
                        if (nChannel == 0)
                            sChName += "B";
                        if (nChannel == 1)
                            sChName += "G";
                        if (nChannel == 2)
                            sChName += "R";
                    }
                    lblIterationInfo.Text = sChName + " " + iteration.ToString();
                    Application.DoEvents();
                }

                //Calculation
                //repaired(i+1) = repaired(i) - tau *
                //         (repaired(i)**psf - o) ** psf + lambda * Divergence(repaired(i)));

                //FT on repaired
                Repaired_FFT = DFT(repaired, null, ref FTReal, ref FTImag, DftFlags.Scale); 
                Cv2.MulSpectrums(Repaired_FFT, PSF_FFT_Squared, Repaired_FFTxPSF_FFT_Squared, DftFlags.None);
                Cv2.Idft(Repaired_FFTxPSF_FFT_Squared, inversedResult);
                Cv2.Split(inversedResult, out outputPlanes);
                Repaired_x_PSF_x_PSF = outputPlanes[0];

                //Calculate divergence
                if (bRegularize)
                    divergence = calcDivergence(nWidth, nHeight, (float)epsilonPow2, repaired);

                //Update repaired
                if (bRegularize)
                    repaired = repaired - tau * (Repaired_x_PSF_x_PSF - Observed_x_PSF + lambda * divergence);
                else
                    repaired = repaired - tau * (Repaired_x_PSF_x_PSF - Observed_x_PSF);
            }

            if (FTReal != null)
                FTReal.Dispose();
            if (FTImag != null)
                FTImag.Dispose();
            if (PSF_FFT_Squared != null)
                PSF_FFT_Squared.Dispose();
            if (inversedResult != null)
                inversedResult.Dispose();
            if (outputPlanes[0] != null)
                outputPlanes[0].Dispose();
            if (outputPlanes[1] != null)
                outputPlanes[1].Dispose();
            if (Repaired_FFT != null)
                Repaired_FFT.Dispose();
            if (Repaired_x_PSF_x_PSF != null)
                Repaired_x_PSF_x_PSF.Dispose();
            if (Repaired_FFTxPSF_FFT_Squared != null)
                Repaired_FFTxPSF_FFT_Squared.Dispose();
            if (divergence != null)
                divergence.Dispose();
            GC.Collect();

            //Final output into Mat (for this channel)
            mbIgnoreIterationsChanged = false;
            return repaired;
        }

        //RICHARDSON-LUCY deconvolution
        private Mat RichardsonLucyDeconvolve_Filter2D(Mat observed, Mat PSF, int nIterationCount, int nChannel, 
            bool bMAP, bool bRegularize)
        {
            StringBuilder sbOut = new StringBuilder();

            int nWidth = observed.Width;
            int nHeight = observed.Height;
            SetUpIterations(observed, nChannel, nIterationCount);
            Mat repaired = new Mat(nHeight, nWidth, mImageTypeInternalChannel, new Scalar(0.0f));
            Mat convolutionResult = new Mat(nHeight, nWidth, mImageTypeInternalChannel, new Scalar(0.0f)); //Disposed
            Mat quotient = new Mat(nHeight, nWidth, mImageTypeInternalChannel, new Scalar(0.0f)); //Disposed

            Mat divergence = new Mat();
            float fLambda = 1e-5f; //Divergence regularisation parameter
            float epsilon = 0.004f; 
            float epsilonPow2 = epsilon * epsilon;

            if (bRegularize)
                divergence = observed.EmptyClone(); //Disposed after loop

            //Make kernel be the non-zero section of the PSF
            Mat kernel = cropMat(PSF);

            //Repaired starts as observed
            repaired = observed.Clone();

            for (int iteration = 0; iteration <= nIterationCount; iteration = iteration + 1)
            {
                if (mbProcessingCancelled)
                {
                    mbIgnoreIterationsChanged = false;
                    break;
                }

                //New estimate = Last est x PSF ** (Observed / Last est ** PSF)
                //Or similarly, Poisson MAP (Maximum a posteriori estimation)  
                //https://www.abtosoftware.com/blog/introduction-to-image-restoration-methods-part-2-iterative-algorithms
                //New estimate = Last est x Exp [ PSF * *(Observed / Last est** PSF) - 1]

                //1 Convolve last repaired with PSF
                Cv2.Filter2D(repaired, convolutionResult, -1, kernel, null, 0, BorderTypes.Reflect);

                //2 Divide observed by step 1 convolution result, watch for 0.0/0.0 = NaN
                Cv2.Divide(observed, convolutionResult, quotient);
                Cv2.PatchNaNs(quotient);

                //3 Convolve this quotient with flipped (horizontally and vertically) PSF
                //Flipped PSF is the same in these symetrical PSFs
                Cv2.Filter2D(quotient, convolutionResult, -1, kernel, null, 0, BorderTypes.Reflect);

                //3a MAP Version
                if (bMAP)
                {
                    convolutionResult = convolutionResult - 1.0f;
                    Cv2.Exp(convolutionResult, convolutionResult);
                }

                //3b Total variation regularisation
                if (bRegularize)
                {
                    divergence = 1.0f + fLambda * calcDivergence(nWidth, nHeight, epsilonPow2, repaired);
                    convolutionResult = convolutionResult.Mul(divergence);
                }

                //4 Multiply last repaired and step 3 convolution result
                repaired = repaired.Mul(convolutionResult);

                if (iteration % mnIterationUIFactor == 0)
                {
                    //Save iteration repaired Into mdRestoredMatEst
                    mDeconvolvedMatIterations[nChannel][iteration / mnIterationUIFactor] = repaired.Clone();
                    string sChName = "";
                    if (nChannel == 0)
                        sChName += "B";
                    if (nChannel == 1)
                        sChName += "G";
                    if (nChannel == 2)
                        sChName += "R";
                    lblIterationInfo.Text = "Ch:" + sChName + " " + iteration.ToString();
                    Application.DoEvents();
                    GC.Collect();
                }
            }

            convolutionResult.Dispose();
            quotient.Dispose();
            kernel.Dispose();
            divergence.Dispose();
            GC.Collect();

            string sOut = sbOut.ToString();

            return repaired;
        }

        private Mat RichardsonLucyDeconvolve_SeperableFilter2D(Mat observed, Mat PSF, int nIterationCount, int nChannel)
        {
            SetUpIterations(observed, nChannel, nIterationCount);
            Mat output = new Mat(mnHeight, mnWidth, mImageTypeInternalChannel, new Scalar(0));
            Mat repaired = new Mat(mnHeight, mnWidth, MatType.CV_64F, new Scalar(0));
            Mat observed_double = new Mat(mnHeight, mnWidth, MatType.CV_64F, new Scalar(0));
            Mat convolutionResult = new Mat(mnHeight, mnWidth, MatType.CV_64F, new Scalar(0));
            Mat quotient = new Mat(mnHeight, mnWidth, MatType.CV_64F, new Scalar(0));

            int nFirstNoZeroElementX = 0;
            int nFirstNoZeroElementY = 0;
            for (int x = 0; x < mnWidth; x++)
            {
                if (PSF.At<float>(mnHeight / 2, x) > 0.0f)
                {
                    nFirstNoZeroElementX = x;
                    break;
                }
            }
            for (int y = 0; y < mnHeight; y++)
            {
                if (PSF.At<float>(y, mnWidth / 2) > 0.0f)
                {
                    nFirstNoZeroElementY = y;
                    break;
                }
            }

            //First convert PSF from float to double and normalise so that PSF sums to 1.0
            Mat PSF_double = new Mat(mnHeight, mnWidth, MatType.CV_64F, new Scalar(0));
            PSF.ConvertTo(PSF_double, MatType.CV_64F);
            Scalar summa = Cv2.Sum(PSF_double);
            PSF_double = PSF_double / summa[0];

            double[] dX = new double[mnWidth - 2 * nFirstNoZeroElementX + 1]; //Kernel centre at mnWidth/2
            double[] dY = new double[mnHeight - 2 * nFirstNoZeroElementY + 1]; //Kernel centre at mnHeight/2
            double dNoOfElements = dX.Length + dY.Length;
            for (int x = nFirstNoZeroElementX; x <= mnWidth - nFirstNoZeroElementX; x++)
            {
                dX[x - nFirstNoZeroElementX] = PSF_double.At<double>(mnHeight / 2, x);
            }
            for (int y = nFirstNoZeroElementY; y < mnHeight - nFirstNoZeroElementY; y++)
            {
                dY[y - nFirstNoZeroElementY] = PSF_double.At<double>(y, mnWidth / 2);
            }
            InputArray ax = InputArray.Create<double>(dX);
            InputArray ay = InputArray.Create<double>(dY);

            //Initialize observed_double as a double in range 0 to 1 using observed ushort Mat
            observed.ConvertTo(observed_double, MatType.CV_64F);

            //Repaired starts as observed
            repaired = observed_double.Clone();

            for (int iteration = 0; iteration <= nIterationCount; iteration = iteration + 1)
            {
                if (mbProcessingCancelled)
                {
                    mbIgnoreIterationsChanged = false;
                    break;
                }

                //1 Convolve last repaired with PSF
                Cv2.SepFilter2D(repaired, convolutionResult, repaired.Type(), ax, ay, null, 0, BorderTypes.Reflect);

                //2 Divide observed by step 1 convolution result
                Cv2.Divide(observed_double, convolutionResult * dNoOfElements, quotient);

                //3 Convolve this quotient with flipped (horizontally and vertically) PSF
                //Flipped PSF is the same in these symetrical PSFs
                Cv2.SepFilter2D(quotient, convolutionResult, repaired.Type(), ax, ay, null, 0, BorderTypes.Reflect);

                //4 Multiply last repaired and step 3 convolution result
                repaired = repaired.Mul(convolutionResult * dNoOfElements);

                if (iteration % mnIterationUIFactor == 0)
                {
                    //Save iteration repaired Into mdRestoredMatEst
                    mDeconvolvedMatIterations[nChannel][iteration / mnIterationUIFactor] = repaired.Clone();
                    string sChName = "";
                    if (nChannel == 0)
                        sChName += "B";
                    if (nChannel == 1)
                        sChName += "G";
                    if (nChannel == 2)
                        sChName += "R";
                    lblIterationInfo.Text = "Ch:" + sChName + " " + iteration.ToString();
                    Application.DoEvents();
                }
            }

            //And change from double to float
            repaired.ConvertTo(output, mImageTypeInternalChannel);

            return output;
        }

        private Mat RichardsonLucyDeconvolve_FourierTransform(Mat observed, Mat PSF, int nIterationCount, 
            int nChannel)
        {
            SetUpIterations(observed, nChannel, nIterationCount);
            Mat repaired = new Mat(mnHeight, mnWidth, mImageTypeInternalChannel, new Scalar(0));

            //Repaired starts as observed
            repaired = observed.Clone();

            //Perform Fourier transform on PSF
            //Normalise so that PSF sums to 1.0
            Scalar summa = Cv2.Sum(PSF);
            PSF = PSF / summa[0];

            Mat PSF_shifted = rearrangeQuadrants(PSF);
            Mat PSF_FTReal = new Mat();
            Mat PSF_FTImag = new Mat();
            Mat PSFFT_shifted = DFT(PSF_shifted, null, ref PSF_FTReal, ref PSF_FTImag, DftFlags.None); //No scale

            Mat repaired_FTReal = new Mat();
            Mat repaired_FTImag = new Mat();
            Mat quotient_FTReal = new Mat();
            Mat quotient_FTImag = new Mat();
            Mat repairedFT = new Mat();
            Mat quotient = new Mat();
            Mat quotientFT = new Mat();
            Mat multipliedResult = new Mat();
            Mat inversedResult = new Mat();
            Mat[] outputPlanes = new Mat[2];

            for (int iteration = 0; iteration <= nIterationCount; iteration = iteration + 1)
            {
                if (mbProcessingCancelled)
                {
                    mbIgnoreIterationsChanged = false;
                    break;
                }
                //1 Convolve last repaired with PSF
                //Fourier transform on last repaired
                repairedFT = DFT(repaired, null, ref repaired_FTReal, ref repaired_FTImag, DftFlags.Scale);
                multipliedResult = PSFFT_shifted.EmptyClone();
                Cv2.MulSpectrums(repairedFT, PSFFT_shifted, multipliedResult, DftFlags.None);
                inversedResult = PSFFT_shifted.EmptyClone();
                Cv2.Idft(multipliedResult, inversedResult);
                outputPlanes = new Mat[2];
                Cv2.Split(inversedResult, out outputPlanes);

                //2 Divide observed by real component of step 1 convolution
                Cv2.Divide(observed, outputPlanes[0], quotient);
                Cv2.PatchNaNs(quotient, 0.0f); //Watch out for 0/0 = NaN

                //3 Convolve this quotient with flipped (horizontally and vertically) PSF
                //Flipped PSF is the same in these symetrical PSFs
                quotientFT = DFT(quotient, null, ref quotient_FTReal, ref quotient_FTImag, DftFlags.Scale);
                multipliedResult = PSFFT_shifted.EmptyClone();
                Cv2.MulSpectrums(quotientFT, PSFFT_shifted, multipliedResult, DftFlags.None);
                inversedResult = PSFFT_shifted.EmptyClone();
                Cv2.Idft(multipliedResult, inversedResult);
                outputPlanes = new Mat[2];
                Cv2.Split(inversedResult, out outputPlanes);

                //4 Multiply last repaired and real component of step 3
                repaired = repaired.Mul(outputPlanes[0]);

                if (iteration % mnIterationUIFactor == 0)
                {
                    //Save iteration repaired Into mdRestoredMatEst
                    mDeconvolvedMatIterations[nChannel][iteration / mnIterationUIFactor] = repaired.Clone();
                    string sChName = "";
                    if (nChannel == 0)
                        sChName += "B";
                    if (nChannel == 1)
                        sChName += "G";
                    if (nChannel == 2)
                        sChName += "R";
                    lblIterationInfo.Text = "Ch:" + sChName + " " + iteration.ToString();
                    GC.Collect();
                    Application.DoEvents();
                }
            }

            return repaired;
        }

        //Misc image functions
        private Mat RotateImage(Mat m, float fAngleDeg, ref int nWidth, ref int nHeight, bool bFill,
             bool bCrop, int nCropWidth, int nCropHeight, bool bMakeSizeEven)
        {
            //Rotate image
            Mat imgRotated = new Mat();
            //fAngleDeg = 10.0f;
            double fAngleDeg_Rad = fAngleDeg / 180.0d * Math.PI;
            Size rot_size = new OpenCvSharp.Size();
            double dRotatedWidth = nWidth * Math.Abs(Math.Cos(fAngleDeg_Rad)) + nHeight * Math.Abs(Math.Sin(fAngleDeg_Rad));
            double dRotatedHeight = nHeight * Math.Abs(Math.Cos(fAngleDeg_Rad)) + nWidth * Math.Abs(Math.Sin(fAngleDeg_Rad));
            int nRotatedWidth = (int)Math.Round(dRotatedWidth, 0);
            int nRotatedHeight = (int)Math.Round(dRotatedHeight, 0);

            if (bMakeSizeEven)
            {
                if (nRotatedWidth % 2 == 1)
                    nRotatedWidth = nRotatedWidth + 1;
                if (nRotatedHeight % 2 == 1)
                    nRotatedHeight = nRotatedHeight + 1;
            }

            rot_size = new OpenCvSharp.Size(nRotatedWidth, nRotatedHeight);

            Point2f fCentre = new Point2f(nWidth / 2.0f, nHeight / 2.0f);
            Mat rot_mat = Cv2.GetRotationMatrix2D(fCentre, fAngleDeg, 1.0d);
            rot_mat.Set<double>(0, 2, rot_mat.At<double>(0, 2) + dRotatedWidth / 2.0d - nWidth / 2.0d);
            rot_mat.Set<double>(1, 2, rot_mat.At<double>(1, 2) + dRotatedHeight / 2.0d - nHeight / 2.0d);

            if (bFill) //Rotated image is bigger than the input to accomodate all the input image area
            {
                nWidth = nRotatedWidth;
                nHeight = nRotatedHeight;
            }

            imgRotated = new Mat(nRotatedHeight, nRotatedWidth, m.Type());
            Cv2.WarpAffine(m, imgRotated, rot_mat, rot_size);

            if (bCrop)
                //Crop central rectangle with desired width and height
                imgRotated = new Mat(imgRotated, new Rect((int)Math.Round((nRotatedWidth - nCropWidth) / 2.0f, 0),
                                (int)Math.Round((nRotatedHeight - nCropHeight) / 2.0f, 0),
                          nCropWidth, nCropHeight));

            return imgRotated;
        }

        private void SaveImage(Mat img, string sFilename)
        {
            img = img * mnImageDepth;
            img.ConvertTo(img, mImageTypeLoaded);
            int[] saveParams = new int[1];
            //saveParams[0] = 1; //No tif compression ?
            Cv2.ImWrite(sFilename, img, saveParams);
        }

        private void btnSavePSF_Click(object sender, EventArgs e)
        {
            if (!mbImageInLoaded)
                return;

            Mat mCurrentPSF = new Mat();
            if (chkInvPSF.Checked)
            {
                if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
                    mCurrentPSF = mPsfInvREPAIRED;
                else
                    mCurrentPSF = mPSFInvHistory[cboCurrent.SelectedIndex];
            }
            else
            {
                if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
                    mCurrentPSF = mPsfREPAIRED;
                else
                    mCurrentPSF = mPSFHistory[cboCurrent.SelectedIndex];
            }
            if (mCurrentPSF == null)
                return;

            mCurrentPSF = cropMat(mCurrentPSF);

            this.Cursor = Cursors.WaitCursor;
            string sFilename = Path.GetDirectoryName( txtImage.Text) + @"\" + "PSF.tif";
            Cv2.ImWrite(sFilename, mCurrentPSF);
            this.Cursor = Cursors.Default;
        }

        public Mat GaussKernel(float fSigma, float fThreshold)
        {
            int nSize = 0;
            float fConstant = 1.0f / fSigma / (float)Math.Sqrt(2.0f * Math.PI);
            float fMax = fConstant;
            while (fConstant * Math.Exp(-2 * nSize * nSize / fSigma / fSigma) > fThreshold * fMax)
                nSize++;
            nSize = nSize + 1;
            float fRadialDistSquared = 0.0f;

            Mat kernel = new Mat(nSize * 2 + 1, nSize * 2 + 1, mImageTypeInternalChannel, new Scalar(0.0f));
            int nMiddle = nSize;
            float fKernelSum = 0.0f;
            float fCurrentVal = 0.0f;
            for (int x = 0; x < nSize * 2 + 1; x++)
            {
                for (int y = 0; y < nSize * 2 + 1; y++)
                {
                    fRadialDistSquared = (x - nMiddle) * (x - nMiddle) + (y - nMiddle) * (y - nMiddle);
                    fCurrentVal = fConstant * (float)Math.Exp(-fRadialDistSquared / fSigma / fSigma / 2);
                    fKernelSum = fKernelSum + fCurrentVal;
                    kernel.Set<float>(y, x, fCurrentVal);
                }
            }

            return kernel / fKernelSum;
        }

        private Mat Convolve(Mat img1, Mat img2, bool bNormalise)
        {
            //The Cv2.Filter2D filtering function actually calculates correlation. 
            //If you have a symmetrical convolution kernel, the mathematical expressions for correlation 
            // and convolution are the same.

            //If the kernel is not symmetric, you must flip the kernel and 
            // set the anchor point to(kernel.cols - anchor.x - 1, kernel.rows - anchor.y - 1).
            //This will calculate the actual convolution.

            //Also, it isn't always necessary that the filtering will happen on the image directly. 
            // If the convolution kernel is large enough, OpenCV will automatically switch to a 
            // discrete Fourier transform based algorithm for speedy execution. 
            Mat mConvolved = img1.EmptyClone();
            Cv2.Filter2D(img1, mConvolved, -1, img2, null, 0, BorderTypes.Reflect);
            //Normalise
            if (bNormalise)
            {
                Scalar summa = Cv2.Sum(mConvolved);
                return mConvolved / summa[0];
            }
            else
                return mConvolved;

            //Or, to force use of fourier transforms
            /*
            Mat img1_FTReal = new Mat();
            Mat img1_FTImag = new Mat();
            Mat img1FT = DFT(img1, null, ref img1_FTReal, ref img1_FTImag, DftFlags.Scale); //Scale, ie divide by no of elements

            //Fourier transform on img2
            Mat img2_shifted = rearrangeQuadrants(img2);
            Mat img2_FTReal = new Mat();
            Mat img2_FTImag = new Mat();
            Mat img2FT_shifted = DFT(img2_shifted, null, ref img2_FTReal, ref img2_FTImag, DftFlags.None); //No scale this time

            //Combine to make the complex CLS filter
            Mat multipliedResult = img1FT.EmptyClone();

            //Multiply
            Cv2.MulSpectrums(img1FT, img2FT_shifted, multipliedResult, DftFlags.None);

            //Inverse FT on product
            Mat inversedResult = img1FT.EmptyClone();
            Cv2.Idft(multipliedResult, inversedResult);

            Mat[] outputPlanes = new Mat[2];
            Cv2.Split(inversedResult, out outputPlanes);

            //Normalise
            if (bNormalise)
            {
                Scalar summa = Cv2.Sum(outputPlanes[0]);  //Real component
                return outputPlanes[0] / summa[0];
            }
            else
                return outputPlanes[0];
            */
        }

        private Mat cropMat(Mat m)
        {
            //Crop Mat m in size, crop where Mat is zero
            int nFirstNonZeroElementX = 0;
            int nFirstNonZeroElementY = 0;
            for (int x = 0; x < m.Width; x++)
            {
                if (m.At<float>(m.Height / 2, x) > 0.0f)
                {
                    nFirstNonZeroElementX = x;
                    break;
                }
            }
            for (int y = 0; y < m.Height; y++)
            {
                if (m.At<float>(y, m.Width / 2) > 0.0f)
                {
                    nFirstNonZeroElementY = y;
                    break;
                }
            }

            //Make cropped Mat
            Rect roi = new Rect(nFirstNonZeroElementX, nFirstNonZeroElementY, m.Width - nFirstNonZeroElementX * 2,
                m.Height - nFirstNonZeroElementY * 2);
            Mat crop = new Mat(m, roi);

            return crop;
        }

        private Mat Gamma(Mat m)
        {
            //Mat m assumed to be 0 to 1 float
            float[] fData = new float[m.Width*m.Height];
            m.GetArray(0, 0, fData);
            for (int i = 0; i < fData.Length; i++)
            {
                if (fData[i] <= 0.0031308f)
                    fData[i] = 12.92f * fData[i];
                else
                    fData[i] = (float)Math.Pow(fData[i], 1.0f / 2.4f) * 1.055f - 0.055f;
            }
            return new Mat(m.Height, m.Width, m.Type(), fData);
        }

        private Mat edgetaper(Mat inputImg, double gamma, double beta, int nEdgeRepairMode,
                double dPSFRadius, Mat PSF, bool bRepairTopBottom)
        {
            int nWidth = inputImg.Cols;
            int nHeight = inputImg.Rows;
            int nSize = nWidth * nHeight;
            Mat outputImg = inputImg.EmptyClone(); //Returned
            int nPSFRadius = (int)Math.Round(dPSFRadius, 0);

            if (nEdgeRepairMode == 1)
            {
                outputImg = inputImg.Clone(); //Returned

                for (int y = 0; y < nHeight; y++)
                {
                    float fFirstVal = inputImg.At<float>(y, nWidth - nPSFRadius); //Going left to right
                    float fLastVal = inputImg.At<float>(y, nPSFRadius);
                    float fDiffPerPx = (fLastVal - fFirstVal) / (nPSFRadius * 2);
                    //Make image wrap round on itself like a cylinder, ie values at x = 0 and x = nWidth - 1 are equal
                    for (int x = nWidth - nPSFRadius; x < nWidth; x++)
                        outputImg.Set<float>(y, x, fFirstVal + (x - (nWidth - nPSFRadius)) * fDiffPerPx);
                    for (int x = 0; x < nPSFRadius; x++)
                        outputImg.Set<float>(y, x, fFirstVal + (x + nPSFRadius) * fDiffPerPx);
                }

                if (bRepairTopBottom)
                {
                    //Make top and bottom borders match
                    for (int x = 0; x < nWidth; x++)
                    {
                        float fFirstVal = inputImg.At<float>(nHeight - nPSFRadius, x); //Going top to bottom
                        float fLastVal = inputImg.At<float>(nPSFRadius, x);
                        float fDiffPerPx = (fLastVal - fFirstVal) / (nPSFRadius * 2);
                        for (int y = nHeight - nPSFRadius; y < nHeight; y++)
                            outputImg.Set<float>(y, x, fFirstVal + (y - (nHeight - nPSFRadius)) * fDiffPerPx);
                        for (int y = 0; y < nPSFRadius; y++)
                            outputImg.Set<float>(y, x, fFirstVal + (y + nPSFRadius) * fDiffPerPx);
                    }
                }
            }

            if (nEdgeRepairMode == 0)
            {
                float[] fw1 = new float[nWidth];
                float dx = (float)(2.0 * Math.PI / nWidth);
                float x = (float)-Math.PI;
                for (int i = 0; i < nWidth; i++)
                {
                    fw1[i] = (float)(0.5 * (Math.Tanh((x + gamma / 2) / beta) - Math.Tanh((x - gamma / 2) / beta)));
                    x = x + dx;
                }
                Mat w1 = new Mat(1, nWidth, mImageTypeInternalChannel, fw1);

                float[] fw2 = new float[nHeight];
                float dy = (float)(2.0 * Math.PI / nHeight);
                float y = (float)-Math.PI;
                for (int i = 0; i < nHeight; i++)
                {
                    fw2[i] = (float)(0.5 * (Math.Tanh((y + gamma / 2) / beta) - Math.Tanh((y - gamma / 2) / beta)));
                    y = y + dy;
                }
                Mat w2 = new Mat(nHeight, 1, mImageTypeInternalChannel, fw2);

                Mat w = w2 * w1;
                Cv2.Multiply(inputImg, w, outputImg);

                w1.Dispose();
                w2.Dispose();
                w.Dispose();
                GC.Collect();
            }

            if (nEdgeRepairMode == 2)
            {
                //Taper right hand side to match the left-hand border value, NOT USED
                int nTaperLength = (int)dPSFRadius;
                int nXStart = nWidth - nTaperLength;
                float fLastVal = 0.0f;
                float fWrapAroundValue = 0.0f;
                float fFrac = 0.0f;
                for (int y = 0; y < nHeight; y++)
                {
                    fWrapAroundValue = outputImg.At<float>(y, 0);
                    fLastVal = outputImg.At<float>(y, nXStart - 1);
                    for (int x = nXStart; x < nWidth; x++)
                    {
                        fFrac = (float)(x - nXStart + 1) / nTaperLength;
                        outputImg.Set<float>(y, x, (1.0f - fFrac) * fLastVal + fFrac * fWrapAroundValue);
                    }
                }
            }

            return outputImg;
        }

        private Mat LinearStretchMinMax(Mat m)
        {
            double dMax = 0.0d;
            double dMin = 0.0d;
            Cv2.MinMaxLoc(m, out dMin, out dMax);
            return (m - dMin) / (dMax - dMin);
        }

        private Mat StretchImagePercentiles(Mat m, int nBlackPercentile, int nWhitePercentile)
        {
            float[] fWhite = new float[mnChannels];
            float[] fBlack = new float[mnChannels];
            Mat mStretched = new Mat();
            float fScale = 0.0f;

            double[] dPercentiles = new double[0];
            double dMeanSDChannel_Mean = 0.0d;
            double dMeanSDChannel_SD = 0.0d;
            double dMeanSDChannel_Med = 0.0d;
            int nPtsIncluded = 0;

            Mat[] mCurrentChannels = new Mat[mnChannels];
            if (mnChannels == 1)
                mCurrentChannels[0] = m;
            else
                mCurrentChannels = Cv2.Split(m);
            for (int nCh = 0; nCh < mnChannels; nCh++)
            {
                mMeanSDChannel(mCurrentChannels[nCh], ref dMeanSDChannel_Mean, ref dMeanSDChannel_SD,
                                ref dMeanSDChannel_Med, ref dPercentiles, 0.000d,
                                true, true, -1.0d, -1.0d, ref nPtsIncluded, 10000);

                while (fBlack[nCh] == 0.0f && nBlackPercentile < dPercentiles.Length)
                {
                    fBlack[nCh] = (float)dPercentiles[nBlackPercentile];
                    nBlackPercentile = nBlackPercentile + 25;
                }

                fWhite[nCh] = (float)dPercentiles[nWhitePercentile];

                //Scale between black and white points
                fScale = 1.0f / (fWhite[nCh] - fBlack[nCh]);
                mCurrentChannels[nCh] =  (mCurrentChannels[nCh] - fBlack[nCh]) * fScale;

            }
            if (mnChannels == 1)
                mStretched = mCurrentChannels[0];
            else
                Cv2.Merge(mCurrentChannels, mStretched);

            return mStretched;
        }

        public void mMeanSDChannel(Mat m, ref double dAvg, ref double dSD, ref double dMed,
                  ref double[] dPercentiles, double dIgnoreTailsForSDCalc, bool bUseMedianForSDCalc,
                  bool bCalcPercentiles, double dSpecificBlack, double dSpecificWhite, ref int nPts, int nPercentileCnt)
        {
            int nWidth = m.Width;
            int nHeight = m.Height;

            float[,] B = new float[nHeight, nWidth];
            m.GetArray(0, 0, B);

            double dTot = 0.0d;
            float[] Channel = new float[nWidth * nHeight];
            double dBlack = -double.MaxValue;
            double dWhite = double.MaxValue;
            if (dSpecificBlack != -1.0d)
                dBlack = dSpecificBlack;
            if (dSpecificWhite != -1.0d)
                dWhite = dSpecificWhite;

            if (bCalcPercentiles)
                dPercentiles = new double[nPercentileCnt + 1];
            int nPtNo = 0;

            float val = 0;
            for (int x = 0; x < nWidth; x++)
            {
                for (int y = 0; y < nHeight; y++)
                {
                    val = B[y, x];
                    if (val >= dBlack && val <= dWhite)
                    {
                        dTot += val;
                        Channel[nPtNo] = val;
                        nPtNo++;
                    }
                }
            }
            nPts = nPtNo;
            dAvg = dTot / nPts;

            if (nPts < nWidth * nHeight)
            {
                float[] ChannelCut = new float[nPts];
                Array.Copy(Channel, ChannelCut, nPts);
                Channel = ChannelCut;
            }

            Array.Sort(Channel);
            if (nPts == 0)
                dMed = 0.0d;
            else
                dMed = Channel[nPts / 2];

            dTot = 0.0d;

            //dIgnoreTailsForSDCalc = 0.01 means ignore first and last percentiles of points in SD calc
            //BUT don't ignore the tails when tabulating the percentiles
            double dSDAvg = dAvg;
            if (bUseMedianForSDCalc)
                dSDAvg = dMed;
            double dPtsFraction = 1.0d - 2 * dIgnoreTailsForSDCalc;
            double dLowerThreshold = 0.0d;
            double dUpperThreshold = 0.0d;
            int nLowerThresholdIndex = (int)Math.Ceiling((nPts * dIgnoreTailsForSDCalc));
            if (nLowerThresholdIndex >= Channel.Length)
                nLowerThresholdIndex = 0;
            if (nLowerThresholdIndex <= 0)
                dLowerThreshold = 0.0d;
            else
                dLowerThreshold = Channel[nLowerThresholdIndex];
            int nUpperThresholdIndex = (int)Math.Floor((nPts * (1.0d - dIgnoreTailsForSDCalc)));
            if (nUpperThresholdIndex >= Channel.Length)
                nUpperThresholdIndex = Channel.Length - 1;
            if (nUpperThresholdIndex <= 0)
                dUpperThreshold = 0.0d;
            else
                dUpperThreshold = Channel[nUpperThresholdIndex];

            if (bCalcPercentiles)
            {
                int nPercIndex = 0;
                double dPercLength = dPercentiles.Length - 1.0d;
                for (int i = 0; i < dPercentiles.Length; i++)
                {
                    nPercIndex = (int)(1.0d * nPts * i / dPercLength);
                    if (nPercIndex < 0)
                        nPercIndex = 0;
                    if (Channel.Length == 0)
                        dPercentiles[i] = 0.0d;
                    else
                    {
                        if (nPercIndex >= Channel.Length)
                            nPercIndex = Channel.Length - 1;
                        dPercentiles[i] = Channel[nPercIndex];
                    }
                }
            }

            for (int x = 0; x < nWidth; x++)
            {
                for (int y = 0; y < nHeight; y++)
                {
                    if (dIgnoreTailsForSDCalc == 0.0d ||
                        (B[y, x] > dLowerThreshold && B[y, x] < dUpperThreshold))
                    {
                        dTot = dTot + (B[y, x] - dSDAvg) * (B[y, x] - dSDAvg);
                    }
                }
            }
            dSD = Math.Sqrt(dTot / (dPtsFraction * nPts));
        }

        //PSF Functions
        private Mat applyPSF(Mat inputImg, Mat PSF)
        {
            return Convolve(inputImg, PSF, false);
        }

        private Mat calcPSFCustom(Size filterSize, double radius)
        {
            Mat psfCustom = new Mat(filterSize, mImageTypeInternalChannel, new Scalar(0));
            int nCentreX = filterSize.Width / 2;
            int nCentreY = filterSize.Height / 2;
            double dR2 = 0.0d;
            ushort[,] filter = new ushort[filterSize.Width, filterSize.Height];

            for (int x = 0; x < filterSize.Width; x++)
            {
                for (int y = 0; y < filterSize.Height; y++)
                {
                    dR2 = 1.0d * (x - nCentreX) * (x - nCentreX) + (y - nCentreY) * (y - nCentreY);
                    if (Math.Sqrt(dR2) < radius)
                        psfCustom.Set<float>(y, x, (float)((1.0d - Math.Sqrt(dR2) / radius)));
                    else
                        psfCustom.Set<float>(y, x, 0.0f);
                }
            }

            Scalar summa = Cv2.Sum(psfCustom);
            return psfCustom / summa[0];
        }

        private Mat PSFCrop(Mat PSF, double dCropPSFAtRadius, bool bNormalise)
        {
            Mat PSF_Cropped = new Mat(PSF.Height, PSF.Width, PSF.Type(), new Scalar(0));

            int nCentreX = PSF.Width / 2;
            int nCentreY = PSF.Height / 2;
            double dR2 = 0.0d;
            //Taper the tails linearly of the PSF from
            // dCropPSFAtRadius * 0.75 to dCropPSFAtRadius * 1.25
            double dReduceTailsByFactor = 1.0d;

            for (int x = 0; x < PSF.Width; x++)
            {
                for (int y = 0; y < PSF.Height; y++)
                {
                    dR2 = 1.0d * (x - nCentreX) * (x - nCentreX) + (y - nCentreY) * (y - nCentreY);
                    if (dR2 < dCropPSFAtRadius * dCropPSFAtRadius * 0.75 * 0.75)
                        PSF_Cropped.Set<float>(y, x, PSF.At<float>(y, x));
                    else if (dR2 < dCropPSFAtRadius * dCropPSFAtRadius * 1.25 * 1.25)
                    {
                        dReduceTailsByFactor = 1.0d - (Math.Sqrt(dR2) - dCropPSFAtRadius * 0.75) / (0.5 * dCropPSFAtRadius);
                        PSF_Cropped.Set<float>(y, x, PSF.At<float>(y, x) * (float)dReduceTailsByFactor);
                    }
                }
            }

            if (bNormalise)
            {
                Scalar summa = Cv2.Sum(PSF_Cropped);
                return PSF_Cropped / summa[0];
            }
            else
                return PSF_Cropped;
        }

        private Mat calcPSFFromMTF(int nWidth, int nHeight, double dFWHM, double dWave, ref double dFWHMPSFPlot)
        {
            Mat psf = new Mat(nHeight, nWidth, mImageTypeInternalChannel, new Scalar(0));

            Mat mMTF = new Mat(1, nWidth, MatType.CV_32F, new Scalar(0.0f));
            int nMax = (int)Math.Round(nWidth / 2.0f * 2.35f / dFWHM, 0);
            float v = 0.0f;
            float fPerfect = 0.0f;
            float fAberation = 0.0f;
            float fMTFAtnMax = 0.0f;
            int nSign = 1;
            for (int i = 0; i < nWidth / 2; i++)
            {
                //Alternative positive/negative, see MTF plot
                //MTF starts at max, goes near zero at nMax (scaling factor), then back to max at nWidth - 1
                if (i % 2 == 0)
                    nSign = 1;
                else
                    nSign = -1;
                v = (float)i / nMax;
                fPerfect = (float)(2.0f / Math.PI * (Math.Acos(v) - v * Math.Sqrt(1 - v * v)));
                fAberation = (float)(1.0f - Math.Pow(dWave / 3.38 / 0.18, 2) * (1 - 4 * (v - 0.5) * (v - 0.5)));

                if (i == nMax - 1)
                    fMTFAtnMax = fPerfect * Math.Abs(fAberation);

                if (i < nMax)
                {
                    mMTF.Set<float>(0, i, fPerfect * Math.Abs(fAberation) * nSign);
                    mMTF.Set<float>(0, nWidth - i - 1, -fPerfect * Math.Abs(fAberation) * nSign);
                }
                else
                {
                    //Straight line to zero at centre
                    float fFrac1 = ((float)i - nMax) / (nWidth / 2 - nMax);
                    float fLinearSection = fMTFAtnMax * (1.0f - fFrac1);
                    mMTF.Set<float>(0, i, fLinearSection * nSign);
                    mMTF.Set<float>(0, nWidth - i - 1, -fLinearSection * nSign);
                }
            }

            //Inverse FT on MTF to get back to PSF
            Mat[] planes_MTF = new Mat[2];
            planes_MTF[0] = mMTF;
            planes_MTF[1] = new Mat(1, nWidth, MatType.CV_32FC1, new Scalar(0.0f));//Zero complex input
            Mat mPlanesMerged = new Mat(1, nWidth, MatType.CV_32FC2, new Scalar(0.0f));
            Cv2.Merge(planes_MTF, mPlanesMerged);
            Mat inversedFT = mPlanesMerged.EmptyClone();
            Cv2.Idft(mPlanesMerged, inversedFT);
            Cv2.Split(inversedFT, out planes_MTF);

            float[] fPSF = new float[nWidth];
            for (int i = 0; i < nWidth; i++)
            {
                fPSF[i] = planes_MTF[0].At<float>(0, i);
            }

            mMTF.Dispose();
            planes_MTF[0].Dispose();
            planes_MTF[1].Dispose();
            mPlanesMerged.Dispose();
            inversedFT.Dispose();

            //Compute PSF Half Max for plot
            float fPSFMax = Math.Abs(fPSF[nWidth / 2]);
            float fPSFLast = 0.0f;
            float fPSFCurrent = 0.0f;
            for (int i = nWidth / 2; i < nWidth ; i++)
            {
                if (dFWHMPSFPlot == 0.0d)
                {
                    if (Math.Abs(fPSF[i]) / fPSFMax < 0.5f)
                    {
                        fPSFLast = Math.Abs(fPSF[i-1]) / fPSFMax; //>0.5f
                        fPSFCurrent = Math.Abs(fPSF[i]) / fPSFMax; //<0.5f
                        dFWHMPSFPlot = 2 * (i - nWidth / 2 - (0.5f - fPSFCurrent) / (fPSFLast - fPSFCurrent));
                        break;
                    }
                }
            }

            //Make PSF using the radial function fRadialPSF
            float[] fRadialPSF = new float[nWidth / 2];
            for (int i = 0; i < nWidth / 2; i++)
            {
                fRadialPSF[i] = Math.Abs(fPSF[nWidth / 2 - i]);
            }

            float fR = 0.0f;
            int nRLower = 0;
            int nRHigher = 0;
            float fRLowerVal = 0.0f;
            float fRHigherVal = 0.0f;
            float fFrac = 0.0f;
            for (int x = 0; x < nWidth; x++)
            {
                for (int y = 0; y < nHeight; y++)
                {
                    fR = (float)Math.Sqrt((x - nWidth / 2) * (x - nWidth / 2) + (y - nHeight / 2) * (y - nHeight / 2));
                    nRLower = (int)Math.Floor(fR);
                    nRHigher = nRLower + 1;
                    fFrac = fR - nRLower;
                    fRLowerVal = 0.0f;
                    fRHigherVal = 0.0f;
                    if (nRLower < nWidth / 2)
                        fRLowerVal = fRadialPSF[nRLower];
                    if (nRHigher < nWidth / 2)
                        fRHigherVal = fRadialPSF[nRHigher];

                    psf.Set<float>(y, x, fRLowerVal + fFrac * (fRHigherVal - fRLowerVal));
                }
            }

            Scalar summa = Cv2.Sum(psf);
            return psf / summa[0];
        }

        private Mat calcPSFCircle(int nWidth, int nHeight, double radius, double feather, bool bGaussian, 
            bool bMoffatPSF, double dMoffatBeta, bool bSymetrical)
        {
            Mat psf = new Mat(nHeight, nWidth, mImageTypeInternalChannel, new Scalar(0));
            Point point = new Point(nWidth / 2, nHeight / 2);
            if (bSymetrical)
                point = new Point((nWidth-1) / 2.0d, (nHeight-1) / 2.0d);

            //Moffat FWHM = 2*Alpha*Sqrt(2^(1/Beta)-1) = Gauss FWHM =  2.35*0.707*radius
            //Lorentz is when Beta = 1, FWHM = 2*Aplha
            double dAlphaMoffat = 2.35 * 0.707 * radius / (2 * Math.Sqrt(Math.Pow(2.0d, 1.0d / dMoffatBeta) - 1.0d));
            double dRadiusAtHalfMax = 2.35 * 0.707 * radius / 2.0d; //Math.Pow(1.0d + dRadiusAtHalfMax * dRadiusAtHalfMax  / dAlphaMoffat / dAlphaMoffat, dMoffatBeta) = 2.0d

            double dCentreX = nWidth / 2;
            double dCentreY = nHeight / 2;
            if (bSymetrical)
            {
                dCentreX = (nWidth-1) / 2.0d;
                dCentreY = (nHeight-1) / 2.0d;
            }
            double dR2 = 0.0d;
            float[,] fPSF = new float[nWidth, nHeight];
            if (bGaussian || bMoffatPSF)
            {
                for (int x = 0; x < nWidth; x++)
                {
                    for (int y = 0; y < nHeight; y++)
                    {
                        dR2 = 1.0d * (x - dCentreX) * (x - dCentreX) + (y - dCentreY) * (y - dCentreY);
                        //radius matches SmartDeblur
                        //Gaussian is exp(-R2 / (2*SigmaSquared) ), so 2*Sigma^2 = radius^2
                        // so, sigma is 0.707*radius, and FWHM =  2.35*sigma, radius at half max = 1.17*sigma, 0.83*radius
                        //ie Exp(-0.69) = 0.5

                        if (bGaussian)
                            psf.Set<float>(y,x, (float)(Math.Exp(-dR2 / radius / radius) ));
                        if (bMoffatPSF)
                            psf.Set<float>(y, x, (float)(1.0d / Math.Pow(1.0d + dR2 / dAlphaMoffat / dAlphaMoffat, dMoffatBeta)));
                    }
                }

            }
            else //Line PSF for motion blur
            {
                int nRadius = (int)Math.Ceiling(radius);
                int nFillCol = 60000;
                double dExtraRadius = radius - nRadius;
                int nExtraFillCol = (int)(dExtraRadius * nFillCol);

                Cv2.Circle(psf, point, nRadius, new Scalar(nFillCol), Cv2.FILLED, LineTypes.AntiAlias);
                Cv2.Circle(psf, point, nRadius + 1, new Scalar(nExtraFillCol), 1, LineTypes.AntiAlias);
                //Cv2.Ellipse(h, point, new Size(radius, radius), 0.0, 0, 360, new Scalar(nFillCol), Cv2.FILLED); //FILLED
            }

            Mat psf_Output = psf.Clone();
            //Feather PSF, useful for motion blur line PSFs
            int nFeather = (int)Math.Round(feather * 2, 0);
            if (nFeather != 0)
            {
                if (nFeather % 2 == 0)
                    nFeather = nFeather + 1;
                Cv2.GaussianBlur(psf, psf_Output, new Size(nFeather, nFeather), 0);
            }

            psf.Dispose();

            //Normalise
            Scalar summa = Cv2.Sum(psf_Output);
            return psf_Output / summa[0];
        }

        private Mat PSFCroppedForDisplay(Mat psf, int nPxWidth, float fScale)
        {
            if (psf == null)
                return null;
            if (psf.Width == 0)
                return null;
            int nYMid = psf.Height / 2;
            int nXMid = psf.Width / 2;
            int nCropDistance = nPxWidth/2;

            double dMax = 0.0d;
            double dMin = 0.0d;
            Cv2.MinMaxIdx(psf, out dMin, out dMax);
            float fRange = (float)(dMax - dMin);
            psf = (psf - dMin) / fRange ;
            float fMax = 1.0f;

            Mat crop = new Mat(nCropDistance *2+1, nCropDistance * 2 + 1, MatType.CV_16UC1);
            for (int nX = - nCropDistance; nX <=  nCropDistance; nX++)
            {
                for (int nY =  -nCropDistance; nY <=  nCropDistance; nY++)
                {
                    crop.Set<ushort>(nY + nCropDistance, nX + nCropDistance,
                        (ushort)(fScale / fMax * psf.At<float>(nYMid + nY, nXMid + nX)));
                }
            }

            return crop;
        }

        private Mat calcPSFMotionBlur(int nWidth, int nHeight, double len, double theta, bool bAntiAlias, double dFeather)
        {
            Mat psf = new Mat(nHeight, nWidth, mImageTypeInternalChannel, new Scalar(0));
            Point point = new Point(nWidth / 2, nHeight / 2);

            //PSF Sums to 1.0
            //An anti-aliased line ONLY works for CV_8U
            Mat h8U = new Mat(nHeight, nWidth, MatType.CV_8U, new Scalar(0));
            int nIntensity = 100;
            int nLineStyle = (int)LineTypes.Link8;
            if (bAntiAlias)
                nLineStyle = (int)LineTypes.AntiAlias;

            double dThetaRad = theta / 180.0d * Math.PI;
            int nCentreX = nWidth / 2;
            int nCentreY = nHeight / 2;

            double dLenX = len / 2.0 * Math.Cos(dThetaRad);
            int nLenX = (int)Math.Floor(dLenX);
            int nStartX = nCentreX - nLenX;
            int nEndX = nCentreX + nLenX;

            double dLenY = len / 2.0 * Math.Sin(dThetaRad);
            int nLenY = (int)Math.Floor(dLenY);
            int nStartY = nCentreY + nLenY;
            int nEndY = nCentreY - nLenY;

            //Extended line for fractional length
            double dExtraLength = Math.Sqrt((dLenX - nLenX) * (dLenX - nLenX) + (dLenY - nLenY) * (dLenY - nLenY));
            int nExtraPixelIntensity = (int)(dExtraLength * nIntensity);
            int nLenXExtended = (int)Math.Ceiling(dLenX);
            int nLenYExtended = (int)Math.Ceiling(dLenY);
            int nStartXExtended = nCentreX - nLenXExtended;
            int nEndXExtended = nCentreX + nLenXExtended;
            int nStartYExtended = nCentreY + nLenYExtended;
            int nEndYExtended = nCentreY - nLenYExtended;
            Cv2.Line(h8U, new Point(nStartXExtended, nStartYExtended), new Point(nStartX, nStartY), new Scalar(nExtraPixelIntensity), int.Parse(txtPSFLineThickness.Text), LineTypes.Link8);
            Cv2.Line(h8U, new Point(nEndX, nEndY), new Point(nEndXExtended, nEndYExtended), new Scalar(nExtraPixelIntensity), int.Parse(txtPSFLineThickness.Text), LineTypes.Link8);

            //Main line at full intensity
            Cv2.Line(h8U, new Point(nStartX, nStartY), new Point(nEndX, nEndY), new Scalar(nIntensity), int.Parse(txtPSFLineThickness.Text), (LineTypes)nLineStyle);
            h8U.ConvertTo(psf, mImageTypeInternalChannel);


            //Draws a line at angle theta to horizontal, with length = len + 1
            //Cv2.Ellipse(h, point, new Size(0, Math.Round((float)len / 2.0, 0)), 90.0 - theta, 0, 360, new Scalar(60000), Cv2.FILLED); //FILLED

            int nFeather = (int)Math.Round(dFeather * 2, 0);
            if (nFeather != 0)
            {
                if (nFeather % 2 == 0)
                    nFeather = nFeather + 1;
                Cv2.GaussianBlur(psf, psf, new Size(nFeather, nFeather), 0);
            }

            Scalar summa = Cv2.Sum(psf);
            return psf / summa[0];
        }

        //Fourier transform functions
        private Mat DFT(Mat realInput, Mat complexInput, ref Mat FTReal, ref Mat FTComplex, DftFlags flags)
        {
            Mat FTOutput = realInput.EmptyClone(); //Returned
            Mat FTInput = realInput.EmptyClone(); //Disposed
            if (complexInput == null)
                complexInput = Mat.Zeros(realInput.Size(), realInput.Type());

            Mat[] inputPlanes = new Mat[2] { realInput, complexInput }; //Disposed
            Cv2.Merge(inputPlanes, FTInput);
            complexInput.Dispose();
            //inputPlanes[0].Dispose(); //Do not dispose, this Mat is passed in
            inputPlanes[1].Dispose();

            Cv2.Dft(FTInput, FTOutput, flags);
            FTInput.Dispose();

            Mat[] outputPlanes = new Mat[2]; //Returned
            Cv2.Split(FTOutput, out outputPlanes);
            FTReal = outputPlanes[0];
            FTComplex = outputPlanes[1];

            GC.Collect();

            return FTOutput;
        }

        private Mat multiplyRealFFTs(Mat FFTComplex, Mat realFFT)
        {
            Mat result = FFTComplex.EmptyClone(); //Returned

            //Split FFTComplex
            Mat[] FFTComplexPlanes = new Mat[2]; //Disposed
            Cv2.Split(FFTComplex, out FFTComplexPlanes);

            //Split realFFT
            Mat[] FFTRealPlanes = new Mat[2]; //Disposed
            Cv2.Split(realFFT, out FFTRealPlanes);

            FFTComplexPlanes[0] = FFTComplexPlanes[0].Mul(FFTRealPlanes[0]);
            FFTComplexPlanes[1] = FFTComplexPlanes[1].Mul(FFTRealPlanes[0]);

            //Merge back to result
            Cv2.Merge(FFTComplexPlanes, result);

            FFTComplexPlanes[0].Dispose();
            FFTComplexPlanes[1].Dispose();
            FFTRealPlanes[0].Dispose();
            FFTRealPlanes[1].Dispose();

            return result;
        }

        private void multiplyRealFFTs(ref double[] outFFT, double[] kernelFFT, int width, int height)
        {
            double value = 0.0d;
            for (int i = 0; i < outFFT.Length; i++)
            {
                value = kernelFFT[(i / 2) * 2]; //The real component of the kernelFFT at this xy position
                outFFT[i] = outFFT[i] * value;
            }
        }

        private double[][] multiplyRealFFTs(double[][] outFFT, double[][] kernelFFT, int width, int height)
        {
            double[][] dProduct = new double[width * height][];
            int index = 0;
            double value = 0.0d;
            for (int i = 0; i < width * height; i++)
            {
                dProduct[i] = new double[2];
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    index = y * width + x;
                    value = kernelFFT[index][0];
                    dProduct[index][0] = outFFT[index][0] * value;
                    dProduct[index][1] = outFFT[index][1] * value;
                }
            }
            return dProduct;
        }

        private Mat CalcFTByDivision(Mat output, Mat input)
        {
            //With a (blurred) input image, and a repaired output image, 
            // calculate what Fourier Transform would have generated the repair, 
            // ie returnFT = FT(output) / FT(input)
            //Actually return the ABS of returnFT so that it can be plotted
            int nChannels = input.Channels();
            if (nChannels > 1)
            {
                Mat[] mCh = new Mat[nChannels];
                mCh = Cv2.Split(input);
                input = mCh[1].Clone(); //Use green channel
                mCh = Cv2.Split(output);
                output = mCh[1].Clone();
                mCh[0].Dispose();
                mCh[1].Dispose();
                mCh[2].Dispose();
            }

            //returnFT = FT(output) / FT(input) 
            //Dividing by the complex array FT(input) can be achieved as follows:
            //returnFT = FT(output) x FT*(input) / ABS(FT(input))^2, where FT* means the complex conjugate
            //  and then take the ABS(returnFT)

            //1) FT(output) 
            Mat FTReal = new Mat(); //Disposed
            Mat FTImag = new Mat(); //Disposed
            Mat FTOutput = new Mat(); //Disposed
            FTOutput = DFT(output, null, ref FTReal, ref FTImag, DftFlags.None);

            //2) FT(input) and ABS(FT(input))^2
            Mat FTInput = new Mat(); //Disposed
            FTInput = DFT(input, null, ref FTReal, ref FTImag, DftFlags.None);
            Mat Abs2 = FTReal.Mul(FTReal) + FTImag.Mul(FTImag);

            //3) Multiply complex FTOutput with conjugate of FTInput
            Mat product = FTOutput.EmptyClone();
            Cv2.MulSpectrums(FTOutput, FTInput, product, DftFlags.None, true); //Conjugate FT*(Input) when multiplying

            //4) ABS of product
            Mat[] planes = Cv2.Split(product); //Get the real and imaginary parts
            Mat returnFT_Real = planes[0] / Abs2;
            Mat returnFT_Imag = planes[1] / Abs2;
            //In case of divide by zero
            Cv2.PatchNaNs(returnFT_Real);
            Cv2.PatchNaNs(returnFT_Imag);
            Mat returnFT = returnFT_Real.Mul(returnFT_Real) + returnFT_Imag.Mul(returnFT_Imag);
            Cv2.Pow(returnFT, 0.5d, returnFT);

            returnFT_Real.Dispose();
            returnFT_Imag.Dispose();
            FTReal.Dispose();
            FTImag.Dispose();
            FTOutput.Dispose();
            FTInput.Dispose();
            product.Dispose();
            Abs2.Dispose();
            planes[0].Dispose();
            planes[1].Dispose();
            if (nChannels > 1)
            {
                input.Dispose();
                output.Dispose();
            }

            return returnFT;
        }

        private Mat PsfFromFT(Mat realFT, Mat imagFT)
        {
            //Calculate the PSF that would have been used to convolve, ie InverseFT(1/(realFT + i x imagFT))
            //1/z = (Conjugate z) / Abs(z)^2
            if (imagFT == null)
                imagFT = new Mat(realFT.Height, realFT.Width, realFT.Type(), new Scalar(0));
            Mat Abs2 = realFT.Mul(realFT) + imagFT.Mul(imagFT);
            Mat[] Planes = new Mat[2];
            Planes[0] = realFT / Abs2;
            Planes[1] = -imagFT / Abs2;

            //In case of divide by zero
            Cv2.PatchNaNs(Planes[0]);
            Cv2.PatchNaNs(Planes[1]);

            Mat reciprocalFT = new Mat(realFT.Height, realFT.Width, MatType.CV_32FC2, new Scalar(0.0));
            Cv2.Merge(Planes, reciprocalFT);

            Mat inversed = reciprocalFT.EmptyClone();
            Cv2.Idft(reciprocalFT, inversed, DftFlags.None);
            Planes = Cv2.Split(inversed);

            //Return the real component of the PSF only
            Mat returnMat = Planes[0];
            returnMat = rearrangeQuadrants(returnMat);

            reciprocalFT.Dispose();
            inversed.Dispose();
            Planes[0].Dispose();
            Planes[1].Dispose();

            double dMax = 0.0f;
            double dMin = 0.0f;
            Cv2.MinMaxLoc(returnMat, out dMin, out dMax);

            return returnMat / dMax;
        }

        private Mat rearrangeQuadrants(Mat inputImg)
        {
            //Rearrange the quadrants of  image so that the origin is at the image center
            // note that inputImg will already have an even number of rows and columns
            Mat outputImg = inputImg.Clone(); //Returned
            int nHalfWidth = outputImg.Cols / 2;
            int nHalfHeight = outputImg.Rows / 2;
            Mat q0 = new Mat(outputImg, new Rect(0, 0, nHalfWidth, nHalfHeight)); // Top-Left - Create a ROI per quadrant
            Mat q1 = new Mat(outputImg, new Rect(nHalfWidth, 0, nHalfWidth, nHalfHeight)); // Top-Right
            Mat q2 = new Mat(outputImg, new Rect(0, nHalfHeight, nHalfWidth, nHalfHeight)); // Bottom-Left
            Mat q3 = new Mat(outputImg, new Rect(nHalfWidth, nHalfHeight, nHalfWidth, nHalfHeight)); // Bottom-Right

            Mat tmp = inputImg.EmptyClone(); //Disposed
            q0.CopyTo(tmp); // swap quadrant (Top-Right with Bottom-Left)
            q3.CopyTo(q0);
            tmp.CopyTo(q3);
            q1.CopyTo(tmp); // swap quadrant (Top-Right with Bottom-Left)
            q2.CopyTo(q1);
            tmp.CopyTo(q2);

            tmp.Dispose();
            q0.Dispose();
            q1.Dispose();
            q2.Dispose();
            q3.Dispose();

            return outputImg;
        }

        //Total variation functions
        private Mat calcTotVariation(int nWidth, int nHeight, float lambda, Mat Img, float epsilon)
        {
            //From DeconvolutionLab  RichardsonLucyTV.java
            //F:\sd\~main\C#\OpenCV\DeconvolutionLab2.pdf
            float[] TV = new float[nWidth * nHeight];

            float[] fImg = new float[nWidth * nHeight];
            Img.GetArray(0, 0, fImg);

            float absGrad = 0.0f;
            float fx = 0.0f;
            float fy = 0.0f;

            float[]GradientX = new float[nWidth * nHeight];
            float[] GradientY = new float[nWidth * nHeight];

            for (int y = 0; y < nHeight - 1; y++)
            {
                for (int x = 0; x < nWidth - 1; x++)
                {
                    int index = y * nWidth + x;

                    GradientX[index] = fImg[index] - fImg[index + 1];
                    GradientY[index] = fImg[index] - fImg[index + nWidth];

                    //Normalize
                    absGrad = (float)Math.Sqrt(GradientX[index] * GradientX[index] + GradientY[index] * GradientY[index]);
                    if (absGrad < epsilon) //Don't allow absGrad to be too small
                    {
                        GradientX[index] = epsilon;
                        GradientY[index] = epsilon;
                    }
                    else
                    {
                        GradientX[index] /= absGrad;
                        GradientY[index] /= absGrad;
                    }
                }
            }

            for (int y = 0; y < nHeight - 1; y++)
            {
                for (int x = 0; x < nWidth - 1; x++)
                {
                    int index = y * nWidth + x;

                    //Gradient of normalised gradient
                    fx = GradientX[index] - GradientX[index + 1];
                    fy = GradientY[index] - GradientY[index + nWidth];
                    TV[index] = 1.0f / (1.0f + lambda * (fx + fy));
                }
            }

            return new Mat(nHeight, nWidth, Img.Type(), TV); ;
        }

        private Mat calcDivergence(int nWidth, int nHeight, float epsilonPow2, Mat Img)
        {
            //Not the same as Cv2.Laplacian(inputMat, laplacian, ddepth, kernel_size, scale, delta);
            //Because of epsilonPow2
            float[] Divergence = new float[nWidth * nHeight];

            float[] fImg = new float[nWidth * nHeight];
            Img.GetArray(0, 0, fImg);

            float[][] GradientMatrix = new float[nWidth * nHeight][];
            for (int i = 0; i < nWidth * nHeight; i++)
                GradientMatrix[i] = new float[2];

            float kValue = 0.0f;
            float fx = 0.0f;
            float fy = 0.0f;

            //Current position a, b is next position in x, c is next postion in y
            //a b
            //c
            //kValue = 1/Sqrt(epsilonPow2 + (b-a)^2 + (c-a)^2)
            //GradientMatrix[at position a][0] = (c-a)/kValue
            //GradientMatrix[at position a][1] = (b-a)/kValue
            //Then
            //fx,fy are gradient of GradientMatrix, sum of fx+fy is divergence
            for (int y = 0; y < nHeight; y++)
            {
                for (int x = 0; x < nWidth; x++)
                {
                    int index = y * nWidth + x;
                    float curValue = fImg[index];
                    if (y < nHeight - 1)
                        GradientMatrix[index][0] = fImg[index + nWidth] - curValue;
                    else
                        GradientMatrix[index][0] = 0;

                    if (x < nWidth - 1)
                        GradientMatrix[index][1] = fImg[index + 1] - curValue;
                    else
                        GradientMatrix[index][1] = 0;

                    // Calculate d
                    kValue = 1 / (float)Math.Sqrt(epsilonPow2 + GradientMatrix[index][0] * GradientMatrix[index][0] + GradientMatrix[index][1] * GradientMatrix[index][1]);

                    GradientMatrix[index][0] = GradientMatrix[index][0] * kValue;
                    GradientMatrix[index][1] = GradientMatrix[index][1] * kValue;

                    // Calculate divergence
                    if (y > 0 && x > 0)
                    {
                        fx = GradientMatrix[index][0] - GradientMatrix[index - nWidth][0];
                        fy = GradientMatrix[index][1] - GradientMatrix[index - 1][1];
                        Divergence[index] = -(fx + fy);
                    }
                }
            }

            return new Mat(nHeight , nWidth, Img.Type(), Divergence);
        }

        private double[] calcDivergence(int nWidth, int nHeight, double epsilonPow2, double[] Img)
        {
            //Not the same as Cv2.Laplacian(inputMat, laplacian, ddepth, kernel_size, scale, delta);
            //Because of epsilonPow2
            double[] Divergence = new double[nWidth * nHeight];
            double[][] GradientMatrix = new double[nWidth * nHeight][];
            for (int i = 0; i < nWidth * nHeight; i++)
                GradientMatrix[i] = new double[2];

            double kValue = 0.0d;
            double fx = 0.0d;
            double fy = 0.0d;

            for (int y = 0; y < nHeight; y++)
            {
                for (int x = 0; x < nWidth; x++)
                {
                    //Build gradient in Img
                    int index = y * nWidth + x;
                    double curValue = Img[index];
                    if (y < nHeight - 1)
                        GradientMatrix[index][0] = Img[index + nWidth] - curValue;
                    else
                        GradientMatrix[index][0] = 0;

                    if (x < nWidth - 1)
                        GradientMatrix[index][1] = Img[index + 1] - curValue;
                    else
                        GradientMatrix[index][1] = 0;

                    // Calculate d
                    kValue = 1 / Math.Sqrt(epsilonPow2 + GradientMatrix[index][0] * GradientMatrix[index][0] + GradientMatrix[index][1] * GradientMatrix[index][1]);

                    GradientMatrix[index][0] = GradientMatrix[index][0] * kValue;
                    GradientMatrix[index][1] = GradientMatrix[index][1] * kValue;

                    // Calculate divergence
                    if (y > 0 && x > 0)
                    {
                        fx = GradientMatrix[index][0] - GradientMatrix[index - nWidth][0];
                        fy = GradientMatrix[index][1] - GradientMatrix[index - 1][1];
                        Divergence[index] = -(fx + fy);
                    }
                }
            }

            return Divergence;
        }

        //Laplacian operator function
        private Mat laplacianOperatorSquared(Mat img, int nx, int ny)
        {
            //Two lines with alternating plus and minus values along  y = 0 and x = 0, following 1/x^2, or 1/y^2
            //Value at (0,0) so that sum of all values is zero
            Mat lapOperator = img.EmptyClone();
            float fSum = 0.0f;
            float fVal = 0.0f;
            float fSign = 1.0f;
            for (int x = 1; x <= nx / 2; x++)
            {
                if (x % 2 == 0)
                    fSign = 1.0f;
                else
                    fSign = -1.0f;
                fVal = fSign * 2.0f / x / x;
                lapOperator.Set<float>(0, x, fVal);
                lapOperator.Set<float>(0, nx - x, fVal);
                fSum = fSum + 2 * fVal;
            }
            for (int y = 1; y <= ny / 2; y++)
            {
                if (y % 2 == 0)
                    fSign = 1.0f;
                else
                    fSign = -1.0f;
                fVal = fSign * 2.0f / y / y;
                lapOperator.Set<float>(y, 0, fVal);
                lapOperator.Set<float>(ny - y, 0, fVal);
                fSum = fSum + 2 * fVal;
            }
            lapOperator.Set<float>(0, 0, -fSum);

            //Now perform a Fourier transform on lapOperator
            Mat lapFFT_Real = new Mat(); //Returned
            Mat lapFFT_Imag = new Mat(); //Disposed
            DFT(lapOperator, null, ref lapFFT_Real, ref lapFFT_Imag, DftFlags.None);
            lapOperator.Dispose();
            lapFFT_Imag.Dispose();
            //And return the square of the real part of the Fourier transform
            return lapFFT_Real.Mul(lapFFT_Real);

            /*
            //ALTERNATIVE METHOD
            //Create the Fourier transfrom array directly (not using a FT)
            //Like in DeconvolutionLab2 Regularized inverse filter (RIF)
            //public static ComplexSignal laplacian  and  public static ComplexSignal createHermitian
            float[] lapOperatorAbs = new float[nx * ny];
            //fnLaplacian = [(1 - Abs(2x/width - 1) )^2 + (1 - Abs(2y/height - 1) )^2] * PI*PI
            //returned Mat = fnLaplacian * fnLaplacian
            float fnLaplacian = 0.0f;
            float fPI2 = (float)(Math.PI * Math.PI);
            for (int y = 0; y < ny; y++)
                for (int x = 0; x < nx; x++)
                {
                    fnLaplacian = (float)(Math.Pow(1.0d - Math.Abs(2.0d * x / nx - 1), 2.0d)
                                + Math.Pow(1.0d - Math.Abs(2.0d * y / ny - 1), 2.0d)) * fPI2;
                    lapOperatorAbs[x + y * nx] = fnLaplacian * fnLaplacian;
                }

            //Create a Mat from the lapOperatorAbs array
            return new Mat(ny, nx, img.Type(), lapOperatorAbs);
            */
        }

        //UI for iterative deconvolution algorithms
        private void SetUpIterations(Mat imgIn, int nChannel, int nIterationCount)
        {
            if (nIterationCount < mnNoOfIterationSteps)
                mnIterationUIFactor = 1;
            else
                mnIterationUIFactor = nIterationCount / mnNoOfIterationSteps; //Max of mnNoOfIterationSteps steps
            mnNoOfIterationsStored = nIterationCount / mnIterationUIFactor + 1;
            mDeconvolvedMatIterations[nChannel] = new Mat[mnNoOfIterationsStored];

            if (nChannel == 0)
            {
                //UI stuff, Only do this once
                mbIgnoreIterationsChanged = true;
                trkIterations.Maximum = nIterationCount / mnIterationUIFactor;

                mbIgnoreIterationsChanged = true;
                trkIterations.Value = trkIterations.Maximum;
                mbIgnoreIterationsChanged = false;
            }
        }

        //Image display functions
        private void MatToPictureBox(Mat mat, PictureBox pic, bool bChangeLocation, System.Drawing.Point newLocation)
        {
            if (mat == null)
            {
                pic.Image = null;
                return;
            }

            //Mat must be CV_8U for BitmapConverter.ToBitmap, Depth 0 and 1 are 8 bit size
            if (mat.Depth() >= 2)
                mat = mat / 256;

            Mat matBmp = new Mat(mat.Height, mat.Width, MatType.CV_8U);
            mat.ConvertTo(matBmp, MatType.CV_8U);

            if (bChangeLocation)
            {
                pic.Image = null;
                pic.Location = newLocation;
            }

            pic.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(matBmp);
            matBmp.Dispose();
            GC.Collect();
        }

        private int mnZoomedImageTopLeftX = 0;
        private int mnZoomedImageTopLeftY = 0;
        private int mnZoomedImageWidth = 0;
        private int mnZoomedImageHeight = 0;
        private void MatToPictureBox_Zoomed(Mat m, PictureBox pic, string sDescription, bool bApplyZoom, 
            bool bResetStartXYPostion, bool bZoomChanged)
        {
            int nPicLocationX = pic.Location.X;
            int nPicLocationY = pic.Location.Y;
            bool bChangePicLocation = false;
            System.Drawing.Point newLocation = new System.Drawing.Point();

            //Convert mat back to original scale
            m = m * mnImageDepth;
            m.ConvertTo(m, mImageTypeLoaded);

            //Desired dimesions
            int nNewWidth = (int)Math.Round(m.Width * mdScale, 0);
            int nNewHeight = (int)Math.Round(m.Height * mdScale, 0);

            if (bApplyZoom)
            {
                if (nNewWidth <= mnMaxImageWidth)
                {
                    mnZoomedImageTopLeftX = 0;
                    mnZoomedImageTopLeftY = 0;
                    mnZoomedImageWidth = mnMaxImageWidth;
                    mnZoomedImageHeight = mnMaxImageWidth;
                    Mat imgTifOutZoomed = new Mat(nNewHeight, nNewWidth, m.Type());
                    Cv2.Resize(m, imgTifOutZoomed, imgTifOutZoomed.Size(), 1.0d, 1.0d, interpolationFlags);
                    MatToPictureBox(imgTifOutZoomed, pic, false, newLocation);
                    imgTifOutZoomed.Dispose();
                    GC.Collect();
                }
                else
                {
                    //Max width allowed when zoomed of mnMaxImageWidth, so crop part of original image before resizing
                    Decimal fImageX = 0.0m;
                    Decimal fImageY = 0.0m;
                    int nStartX = 0;
                    int nStartY = 0;
                    int nEndX = 0;
                    int nEndY = 0;
                    if (bResetStartXYPostion)
                    {
                        Decimal dZoom = mdScale;
                        if (bZoomChanged)
                            dZoom = mdLastScale;

                        PicBoxXY_ToImagePixelXY(pic, dZoom, pnlOut.Width / 2.0m, pnlOut.Height / 2.0m, ref fImageX,
                            ref fImageY, mnZoomedImageTopLeftX, mnZoomedImageTopLeftY);

                        //Crop the original image so that image co-ords (fImageX, fImageY), ie, at the centre of the picture panel, 
                        // are at centre of cropped image
                        mnZoomedImageWidth = (int)(mnMaxImageWidth / mdScale);
                        mnZoomedImageHeight = mnZoomedImageWidth;
                        nStartX = (int)((float)fImageX - mnZoomedImageWidth / 2);
                        if (nStartX < 0)
                        {
                            nStartX = 0;
                            nEndX = nStartX + mnZoomedImageWidth;
                        }
                        else
                        {
                            nEndX = nStartX + mnZoomedImageWidth;
                        }
                        if (nEndX >= mnWidth)
                        {
                            nEndX = mnWidth - 1;
                            nStartX = nEndX - mnZoomedImageWidth;
                            if (nStartX < 0)
                                nStartX = 0;
                        }

                        nStartY = (int)((float)fImageY - mnZoomedImageHeight / 2);
                        if (nStartY < 0)
                        {
                            nStartY = 0;
                            nEndY = nStartY + mnZoomedImageWidth;
                        }
                        else
                        {
                            nEndY = nStartY + mnZoomedImageWidth;
                        }
                        if (nEndY >= mnHeight)
                        {
                            nEndY = mnHeight - 1;
                            nStartY = nEndY - mnZoomedImageWidth;
                            if (nStartY < 0)
                                nStartY = 0;
                        }

                        if (!bZoomChanged) //When panning only
                        {
                            //Adjust the pic location, as mnZoomedImageTopLeftX and mnZoomedImageTopLeftY are about to change
                            bChangePicLocation = true;
                            newLocation = new System.Drawing.Point(picOut.Location.X + (int)Math.Round((nStartX - mnZoomedImageTopLeftX) * mdScale, 0),
                                picOut.Location.Y + (int)Math.Round((nStartY - mnZoomedImageTopLeftY) * mdScale, 0));
                        }

                        mnZoomedImageTopLeftX = nStartX;
                        mnZoomedImageTopLeftY = nStartY;
                        mnZoomedImageWidth = nEndX - nStartX;
                        mnZoomedImageHeight = nEndY - nStartY;
                    }

                    //Crop the current mat and resize
                    Mat mCropped = new Mat(m, new Rect(mnZoomedImageTopLeftX, mnZoomedImageTopLeftY,
                        mnZoomedImageWidth, mnZoomedImageHeight));
                    nNewWidth = (int)Math.Round(mnZoomedImageWidth * mdScale, 0);
                    nNewHeight = (int)Math.Round(mnZoomedImageHeight * mdScale, 0);
                    Mat imgTifOutZoomed = new Mat(nNewHeight, nNewWidth, m.Type());
                    Cv2.Resize(mCropped, imgTifOutZoomed, imgTifOutZoomed.Size(), 1.0d, 1.0d, interpolationFlags);
                    mCropped.Dispose();
                    MatToPictureBox(imgTifOutZoomed, pic, bChangePicLocation, newLocation);
                    imgTifOutZoomed.Dispose();
                    GC.Collect();
                }
            }
            else
            {
                mnZoomedImageTopLeftX = 0;
                mnZoomedImageTopLeftY = 0;
                mnZoomedImageWidth = 0;
                mnZoomedImageHeight = 0;
                MatToPictureBox(m, pic, false, newLocation);
            }
            if (sDescription != "")
                lblProcessing.Text = sDescription;
        }

        private void PlotMTF(Mat psf, float fFWHM)
        {
            if (!optMTF.Checked)
                return;
            if (fFWHM == 0.0f)
                return;

            System.Drawing.Graphics g = picFilterFT.CreateGraphics();
            g.Clear(picFilterFT.BackColor);
            if (psf == null)
                return;
            if (psf.Width == 0)
                return;

            //1D psf
            int nHalfHeight = psf.Height / 2;
            int nWidth = psf.Width;
            Mat psf1D = new Mat(1, nWidth, psf.Type());
            for (int x = 0; x < nWidth; x++)
            {
                psf1D.Set<float>(0, x, psf.At<float>(nHalfHeight, x));
            }
            Mat PSF_FTReal = new Mat(); //Disposed
            Mat PSF_FTImag = new Mat(); //Disposed
            //Fourier transform of 1D PSF
            DFT(psf1D, null, ref PSF_FTReal, ref PSF_FTImag, DftFlags.None);

            float fMax = Math.Abs(PSF_FTReal.At<float>(0, 0));
            int nMax = (int)Math.Round(nWidth / 2.0f * 2.35f / fFWHM, 0);

            int nPicWidth = picFilterFT.Width;
            int nPicHeight = picFilterFT.Height;

            System.Drawing.PointF[] pts = new System.Drawing.PointF[nMax];
            float fMTF = 0.0f;
            float fPicXPos = 0.0f;
            float fPicYPos = 0.0f;
            for (float fx = 0.0f; fx <= nPicWidth; fx = fx + nPicWidth / 5.0f)
            {
                g.DrawLine(System.Drawing.Pens.DarkGray, fx, 0.0f, fx, nPicHeight);
            }
            for (float fy = 0.0f; fy <= nPicHeight; fy = fy + nPicHeight / 5.0f)
            {
                g.DrawLine(System.Drawing.Pens.DarkGray, 0.0f, fy, nPicWidth, fy);
            }

            for (int x = 0; x < nMax; x++)
            {
                fPicXPos = (float)x / nMax * nPicWidth;
                fMTF = Math.Abs(PSF_FTReal.At<float>(0, x)) / fMax;
                fPicYPos = (1.0f - fMTF) * nPicHeight;

                pts[x] = new System.Drawing.PointF(fPicXPos, fPicYPos);
            }
            try
            {
                g.DrawCurve(System.Drawing.Pens.Black, pts);
            }
            catch (Exception ex)
            {

            }

            psf1D.Dispose();
            PSF_FTReal.Dispose();
            PSF_FTImag.Dispose();
        }

        private void PlotPSFProfile(Mat psf, PictureBox picPSFProfile, int nWidth, float fFWHM)
        {
            System.Drawing.Graphics g = picPSFProfile.CreateGraphics();
            g.Clear(picPSFProfile.BackColor);
            if (psf == null)
                return;
            if (psf.Width == 0)
                return;
            int nPSFWidth = psf.Width;
            int nPSFHeight = psf.Height;
            int picWidth = picPSFProfile.Width;

            if (chkInvPSF.Checked)
                fFWHM = 0.0f;

            double dMax = 0.0d;
            double dMin = 0.0d;
            Point lMax = new Point();
            Point lMin = new Point();
            Cv2.MinMaxLoc(psf, out dMin, out dMax);
            Cv2.MinMaxLoc(psf, out lMin, out lMax);
            float fMax = (float)dMax;

            int nXAxisHeight = 2;
            //PSF can go negative
            if (dMin < 0.0d)
                nXAxisHeight = (int)(picPSFProfile.Height * -dMin / (dMax - dMin)) + 2;
            int picHeight = picPSFProfile.Height - nXAxisHeight;

            System.Drawing.PointF[] pts = new System.Drawing.PointF[nWidth];
            for (int nX = -nWidth/2; nX < nWidth/2; nX++)
            {
                if (nX + nPSFWidth / 2 < 0 || nX + nPSFWidth / 2 >= nPSFWidth)
                    continue;

                pts[nX + nWidth/2] = new System.Drawing.PointF((float)(nX + nWidth/2) / nWidth * picWidth,
                    picHeight - psf.At<float>(lMax.Y, nX + lMax.X) / fMax * picHeight);
            }
            g.DrawCurve(System.Drawing.Pens.Black, pts);
            //Base line
            g.DrawLine(System.Drawing.Pens.DarkGray, 0.0f, picPSFProfile.Height - nXAxisHeight, picPSFProfile.Width, picPSFProfile.Height - nXAxisHeight);
            //Draw FWHM line
            if (fFWHM != 0.0f)
                g.DrawLine(System.Drawing.Pens.Black, picWidth / 2.0f - fFWHM / nWidth * picWidth / 2.0f, picHeight / 2.0f,
                    picWidth / 2.0f + fFWHM / nWidth * picWidth / 2.0f, picHeight / 2.0f);
        }

        private void mDisplayFFTInPic(Mat mat, PictureBox pic, bool bConvertTo16U, double dMin, double dMax)
        {
            if (mat == null)
            {
                pic.Image = null;
                return;
            }
            int nHeight = mat.Height;
            int nWidth = mat.Width;
            Mat mDisplay = mDisplayFFT(nHeight, nWidth, mat, bConvertTo16U, dMin, dMax);
            MatToPictureBox(mDisplay, pic, false, new System.Drawing.Point());
        }

        private void udMaxPlotFT_ValueChanged(object sender, EventArgs e)
        {
            if (optFilterFourierTransform.Checked)
            {
                Mat mCurrentFTDisplayed = null;
                if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
                    mCurrentFTDisplayed = mFTDisplayREPAIRED;
                else
                    mCurrentFTDisplayed = mFTDisplayHistory[cboCurrent.SelectedIndex];

                mDisplayFFTInPic(mCurrentFTDisplayed, picFilterFT, true, 0.0d, (double)udMaxPlotFT.Value);
            }
        }

        private Mat mLogFFT(Mat real, Mat imag)
        {
            //Output image greyscale value = log(1 + sqrt( (Real FT)^2 + (Imag FT)^2) )
            Mat displayFT = new Mat();

            Cv2.Magnitude(real, imag, displayFT);
            Mat matOfOnes = Mat.Ones(displayFT.Size(), MatType.CV_32F); //Add 1.0 to each pixel value
            Cv2.Add(matOfOnes, displayFT, displayFT);
            Cv2.Log(displayFT, displayFT);  // switch to logarithmic scale
            // Transform the output image with float values into a range of 0 to 1
            Cv2.Normalize(displayFT, displayFT, 0, 1.0f, NormTypes.MinMax, real.Type());

            displayFT = rearrangeQuadrants(displayFT);
            return displayFT;
        }

        private Mat mDisplayFFT(int nHeight, int nWidth, Mat mat, bool bConvertTo16U, double dMin, double dMax)
        {
            Point lMin = new Point();
            Point lMax = new Point();
            //Use min and max supplied
            if (dMin == 0.0d && dMax == 0.0d)
                mat.MinMaxIdx(out dMin, out dMax);
            //mat.MinMaxLoc(out lMin, out lMax);

            Mat FilterDiplay = mat.Clone();
            if (bConvertTo16U)
            {
                FilterDiplay = (FilterDiplay - dMin) / (dMax - dMin) * 65000;
                FilterDiplay.ConvertTo(FilterDiplay, MatType.CV_16U);
            }
            else
            {
                FilterDiplay = (FilterDiplay - dMin) / (dMax - dMin);
            }
            FilterDiplay = rearrangeQuadrants(FilterDiplay);
            return FilterDiplay;
        }

        //Image splitting and recombining
        private Mat ImageCombine(Mat[,] imgIn, Mat[,] imgMask, int nOverlap, int nWidth, int nHeight)
        {
            Mat outImg = new Mat(nHeight, nWidth, imgIn[0, 0].Type()); //Maybe 3 channels
            Mat outWgts = new Mat(nHeight, nWidth, imgIn[0, 0].Type()); //Maybe 3 channels
            int nTilesX = imgIn.GetLength(0);
            int nTilesY = imgIn.GetLength(1);
            int nStartX = 0;
            int nStartY = 0;
            int nTileWidth = imgIn[0, 0].Width - nOverlap;
            int nTileHeight = imgIn[0, 0].Height - nOverlap;
            Vec3f vImgVal;
            Vec3f vCurrentOutputVal;
            Vec3f vMask;

            float fImgVal;
            float fCurrentOutputVal;
            float fMaskVal = 0.0f;

            for (int xTileNo = 0; xTileNo < nTilesX; xTileNo++)
            {
                if (xTileNo == 0)
                    nStartX = 0;
                else
                    nStartX = nTileWidth * xTileNo - nOverlap;

                for (int yTileNo = 0; yTileNo < nTilesY; yTileNo++)
                {
                    if (yTileNo == 0)
                        nStartY = 0;
                    else
                        nStartY = nTileHeight * yTileNo - nOverlap;

                    for (int x = nStartX; x < nStartX + imgIn[xTileNo, yTileNo].Width; x++)
                    {
                        for (int y = nStartY; y < nStartY + imgIn[xTileNo, yTileNo].Height; y++)
                        {
                            if (mnChannels == 1)
                            {
                                fCurrentOutputVal = outImg.At<float>(y, x);
                                fImgVal = imgIn[xTileNo, yTileNo].At<float>(y - nStartY, x - nStartX);
                                fMaskVal = imgMask[xTileNo, yTileNo].At<float>(y - nStartY, x - nStartX);
                                outImg.Set<float>(y, x, fCurrentOutputVal + fImgVal * fMaskVal);
                                outWgts.Set<float>(y, x, outWgts.At<float>(y, x) + fMaskVal);
                            }
                            else
                            {
                                vCurrentOutputVal = outImg.At<Vec3f>(y, x);
                                vImgVal = imgIn[xTileNo, yTileNo].At<Vec3f>(y - nStartY, x - nStartX);
                                fMaskVal = imgMask[xTileNo, yTileNo].At<float>(y - nStartY, x - nStartX);
                                vMask = outWgts.At<Vec3f>(y, x);
                                for (int ch = 0; ch < mnChannels; ch++)
                                {
                                    vCurrentOutputVal[ch] = vCurrentOutputVal[ch] + vImgVal[ch] * fMaskVal;
                                    vMask[ch] = vMask[ch] + fMaskVal;
                                }
                                outImg.Set<Vec3f>(y, x, vCurrentOutputVal);
                                outWgts.Set<Vec3f>(y, x, vMask);
                            }
                        }
                    }
                }
            }

            outImg = outImg / outWgts;

            return outImg;
        }

        //Functions to allow splitting image into tiles and recombining
        private int OverlapLength(int nWidth, int nHeight, float fFieldRotation, int nTiles)
        {
            int nOverlap = 0;

            int nTileWidth = nWidth / nTiles;
            if (nTileWidth % 2 == 1)
                nTileWidth = nTileWidth - 1;

            //Maximum length of star trails
            float fTrailLength = (float)(Math.Sqrt(nWidth * nWidth / 4 + nHeight * nHeight / 4)
                * fFieldRotation / 180.0f * Math.PI);
            nOverlap = nTileWidth / 5;
            if (nOverlap < fTrailLength * 3)
                nOverlap = (int)(fTrailLength * 3);
            if (nOverlap > nTileWidth / 2)
                nOverlap = nTileWidth / 2;

            //Make overlap be even
            if (nOverlap % 2 == 1)
                nOverlap++;

            return nOverlap;
        }

        private Mat[,] ImageSplit(Mat imgIn, int nTilesWide, ref Mat[,] imgMask,
            ref int[,] tileCentreX, ref int[,] tileCentreY, ref int nOverlap, float fFieldRotation)
        {
            int nWidth = imgIn.Width;
            int nHeight = imgIn.Height;

            int nTilesX = nTilesWide;
            int nTilesY = nTilesWide * nHeight / nWidth;
            if (nTilesY < 2)
                nTilesY = 2;

            Mat[,] imgSplit = new Mat[nTilesX, nTilesY];
            tileCentreX = new int[nTilesX, nTilesY];
            tileCentreY = new int[nTilesX, nTilesY];
            imgMask = new Mat[nTilesX, nTilesY];

            int nTileWidth = nWidth / nTilesX;
            if (nTileWidth % 2 == 1)
                nTileWidth = nTileWidth - 1;

            int nTileHeight = nHeight / nTilesY;
            if (nTileHeight % 2 == 1)
                nTileHeight = nTileHeight - 1;

            nOverlap = OverlapLength(nWidth, nHeight, fFieldRotation, nTilesX);
            int nFullyMaskedOverlap = nOverlap  / 3; //This section is needed to avoid edge artefacts in Wiener etc
            int nTaperedOverlap = nOverlap - nFullyMaskedOverlap;

            int nTileLastWidth = nWidth - nTileWidth * (nTilesX - 1);
            int nTileLastHeight = nHeight - nTileHeight * (nTilesY - 1);

            int nCurrentTileWidth = 0;
            int nCurrentTileHeight = 0;
            int nStartX = 0;
            int nStartY = 0;

            for (int xTileNo = 0; xTileNo < nTilesX; xTileNo++)
            {
                if (xTileNo == 0)
                {
                    nStartX = 0;
                    nCurrentTileWidth = nTileWidth + nOverlap;
                }
                else if (xTileNo == nTilesX - 1)
                {
                    nStartX = nTileWidth * xTileNo - nOverlap;
                    nCurrentTileWidth = nTileLastWidth + nOverlap;
                }
                else
                {
                    nStartX = nTileWidth * xTileNo - nOverlap;
                    nCurrentTileWidth = nTileWidth + nOverlap * 2;
                }

                for (int yTileNo = 0; yTileNo < nTilesY; yTileNo++)
                {
                    if (yTileNo == 0)
                    {
                        nStartY = 0;
                        nCurrentTileHeight = nTileHeight + nOverlap;
                    }
                    else if (yTileNo == nTilesY - 1)
                    {
                        nCurrentTileHeight = nTileLastHeight + nOverlap;
                        nStartY = nTileHeight * yTileNo - nOverlap;
                    }
                    else
                    {
                        nCurrentTileHeight = nTileHeight + nOverlap * 2;
                        nStartY = nTileHeight * yTileNo - nOverlap;
                    }

                    //Copy imgIn to a tile in the imgSplit array
                    imgSplit[xTileNo, yTileNo] = new Mat(imgIn, new Rect(nStartX, nStartY, nCurrentTileWidth, nCurrentTileHeight));

                    if (xTileNo == nTilesX - 1)
                        tileCentreX[xTileNo, yTileNo] = xTileNo * nTileWidth + nTileLastWidth / 2;
                    else
                        tileCentreX[xTileNo, yTileNo] = xTileNo * nTileWidth + nTileWidth / 2;
                    if (yTileNo == nTilesY - 1)
                        tileCentreY[xTileNo, yTileNo] = yTileNo * nTileHeight + nTileLastHeight / 2;
                    else
                        tileCentreY[xTileNo, yTileNo] = yTileNo * nTileHeight + nTileHeight / 2;

                    imgMask[xTileNo, yTileNo] = new Mat(nCurrentTileHeight, nCurrentTileWidth, 
                        mImageTypeInternalChannel, new Scalar(1.0f));

                    //Weight = Distance in pixels from nearest edge to Left/Right/Up/Down
                    //Left border
                    if (xTileNo != 0)
                    {
                        for (int x = 0; x < nOverlap; x++)
                        {
                            for (int y = 0; y < nCurrentTileHeight; y++)
                            {
                                if (x < nFullyMaskedOverlap)
                                    imgMask[xTileNo, yTileNo].Set<float>(y, x, 0.0f);
                                else
                                    imgMask[xTileNo, yTileNo].Set<float>(y, x, (float)(x- nFullyMaskedOverlap) / nTaperedOverlap);
                            }
                        }
                    }

                    //Right border
                    if (xTileNo != nTilesX - 1)
                    {
                        for (int x = nCurrentTileWidth - nOverlap; x < nCurrentTileWidth; x++)
                        {
                            for (int y = nCurrentTileHeight - 1; y >= 0; y--)
                            {
                                if (nCurrentTileWidth - x < nFullyMaskedOverlap)
                                    imgMask[xTileNo, yTileNo].Set<float>(y, x, 0.0f);
                                else
                                    imgMask[xTileNo, yTileNo].Set<float>(y, x, (float)(nCurrentTileWidth - nFullyMaskedOverlap - x) / nTaperedOverlap);
                            }
                        }
                    }

                    float fExistingVal = 0.0f;
                    //Top border
                    if (yTileNo != 0)
                    {
                        for (int y = 0; y < nOverlap; y++)
                        {
                            for (int x = 0; x < nCurrentTileWidth; x++)
                            {
                                fExistingVal = imgMask[xTileNo, yTileNo].At<float>(y, x);

                                if (y < nFullyMaskedOverlap)
                                    imgMask[xTileNo, yTileNo].Set<float>(y, x, 0.0f);
                                else if ((float)(y - nFullyMaskedOverlap) / nTaperedOverlap < fExistingVal)
                                    imgMask[xTileNo, yTileNo].Set<float>(y, x, (float)(y - nFullyMaskedOverlap) / nTaperedOverlap);
                            }
                        }
                    }

                    //Bottom border
                    if (yTileNo != nTilesY - 1)
                    {
                        for (int y = nCurrentTileHeight - nOverlap; y < nCurrentTileHeight; y++)
                        {
                            for (int x = 0; x < nCurrentTileWidth; x++)
                            {
                                fExistingVal = imgMask[xTileNo, yTileNo].At<float>(y, x);

                                if (nCurrentTileHeight - y < nFullyMaskedOverlap)
                                    imgMask[xTileNo, yTileNo].Set<float>(y, x, 0.0f);
                                else if ((float)(nCurrentTileHeight - nFullyMaskedOverlap - y - 1) / nTaperedOverlap < fExistingVal)
                                    imgMask[xTileNo, yTileNo].Set<float>(y, x, (float)(nCurrentTileHeight - nFullyMaskedOverlap - y) / nTaperedOverlap);
                            }
                        }
                    }

                    /*
                    //Adjust corners
                    //Top left
                    if (xTileNo != 0 && yTileNo != 0)
                    {
                        for (int x = 0; x < nOverlap; x++)
                        {
                            for (int y = 0; y < nOverlap; y++)
                            {
                                imgMask[xTileNo, yTileNo].Set<float>(y, x, imgMask[xTileNo, yTileNo].At<float>(y, x) / 2.0f);
                            }
                        }
                    }

                    //Top right
                    if (xTileNo != nTilesX - 1 && yTileNo != 0)
                    {
                        for (int x = nCurrentTileWidth - nOverlap + 1; x < nCurrentTileWidth; x++)
                        {
                            for (int y = 0; y < nOverlap; y++)
                            {
                                imgMask[xTileNo, yTileNo].Set<float>(y, x, imgMask[xTileNo, yTileNo].At<float>(y, x) / 2.0f);
                            }
                        }
                    }

                    //Bottom left
                    if (xTileNo != 0 && yTileNo != nTilesY - 1)
                    {
                        for (int x = 0; x < nOverlap; x++)
                        {
                            for (int y = nCurrentTileHeight - nOverlap + 1; y < nCurrentTileHeight; y++)
                            {
                                imgMask[xTileNo, yTileNo].Set<float>(y, x, imgMask[xTileNo, yTileNo].At<float>(y, x) / 2.0f);
                            }
                        }
                    }

                    //Bottom right
                    if (xTileNo != nTilesX - 1 && yTileNo != nTilesY - 1)
                    {
                        for (int x = nCurrentTileWidth - nOverlap + 1; x < nCurrentTileWidth; x++)
                        {
                            for (int y = nCurrentTileHeight - nOverlap + 1; y < nCurrentTileHeight; y++)
                            {
                                imgMask[xTileNo, yTileNo].Set<float>(y, x, imgMask[xTileNo, yTileNo].At<float>(y, x) / 2.0f);
                            }
                        }
                    }
                    */
                }
            }

            return imgSplit;
        }

        //Settings for deconvultion and sharpening layers
        private void mLoadSettings()
        {
            mbSettingsLoaded = false;
            if (!File.Exists("settings.txt"))
            {
                mbIgnoreKernelSharpeningLayersChanged = true;
                cboKernelSharpeningLayers.SelectedIndex = 0;
                mbIgnoreKernelSharpeningLayersChanged = false;
            }
            else
            {
                string[] sSettings = File.ReadAllText("settings.txt").Split(new char[] { '|' });
                int nSettingsIndex = 0;
                txtImage.Text = sSettings[nSettingsIndex++];
                if (sSettings[nSettingsIndex++] == "y")
                    chkAutostretch.Checked = true;
                else
                    chkAutostretch.Checked = false;
                if (sSettings[nSettingsIndex++] == "y")
                    chkAutoDeblur.Checked = true;
                else
                    chkAutoDeblur.Checked = false;
                mbIgnoreZoomChange = true;
                udZoom.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                mbIgnoreZoomChange = false;
                if (sSettings[nSettingsIndex] == "1")
                    optOriginal.Checked = true;
                else if (sSettings[nSettingsIndex] == "2")
                    optPrevious.Checked = true;
                nSettingsIndex++;

                //Repair method
                trkBlur.Value = int.Parse(sSettings[nSettingsIndex++]);
                if (sSettings[nSettingsIndex] == "1")
                    optWiener.Checked = true;
                else if (sSettings[nSettingsIndex] == "2")
                    optTikhonov.Checked = true;
                else if (sSettings[nSettingsIndex] == "3")
                    optLandweber.Checked = true;
                else if (sSettings[nSettingsIndex] == "4")
                    optLR.Checked = true;
                else if (sSettings[nSettingsIndex] == "5")
                    optRIF.Checked = true;
                else
                    optCustomRepair.Checked = true;
                nSettingsIndex++;
                txtNSR.Text = sSettings[nSettingsIndex++];
                txtCLS_Y.Text = sSettings[nSettingsIndex++];
                txtIterations.Text = sSettings[nSettingsIndex++];

                //PSF Display width
                udPSFPlotWidth.Value = Decimal.Parse(sSettings[nSettingsIndex++]);

                //Cosmetic repairs
                if (sSettings[nSettingsIndex++] == "y")
                    chkRepairEdges.Checked = true;
                else
                    chkRepairEdges.Checked = false;
                if (sSettings[nSettingsIndex++] == "y")
                    chkTanhRepair.Checked = true;
                else
                    chkTanhRepair.Checked = false;
                if (sSettings[nSettingsIndex++] == "y")
                    chkRepairTopBottom.Checked = true;
                else
                    chkRepairTopBottom.Checked = false;
                txtGamma.Text = sSettings[nSettingsIndex++];
                txtBeta.Text = sSettings[nSettingsIndex++];
                if (sSettings[nSettingsIndex++] == "y")
                    chkDeringing.Checked = true;
                else
                    chkDeringing.Checked = false;
                udDeringStarThreshold.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udDeringRepairStrength.Value = Decimal.Parse(sSettings[nSettingsIndex++]);

                //Feather
                txtFeather.Text = sSettings[nSettingsIndex++];

                //PSF Repair mode
                if (sSettings[nSettingsIndex] == "1")
                    optCircularBlur.Checked = true;
                else if (sSettings[nSettingsIndex] == "2")
                    optMotionBlur.Checked = true;
                else 
                    chkFiledRotationDeblur.Checked = true;
                nSettingsIndex++;

                //Circular PSF
                udFWHM.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udBrightness.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                if (sSettings[nSettingsIndex++] == "y")
                    chkCropPSF.Checked = true;
                else
                    chkCropPSF.Checked = false;
                udCropPSF.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                if (sSettings[nSettingsIndex] == "1")
                    optVoigt.Checked = true;
                else if (sSettings[nSettingsIndex] == "2")
                    optMoffat.Checked = true;
                else if (sSettings[nSettingsIndex] == "3")
                    optGaussianDeblur.Checked = true;
                else if (sSettings[nSettingsIndex] == "4")
                    optCameraCircleDeblur.Checked = true;
                else if (sSettings[nSettingsIndex] == "5")
                    optMTFPSF.Checked = true;
                nSettingsIndex++;
                udGaussFraction.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udMoffatBeta.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                if (sSettings[nSettingsIndex++] == "y")
                    chkMoffatInPasses.Checked = true;
                else
                    chkMoffatInPasses.Checked = false;
                udMoffatPasses.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udWave.Value = Decimal.Parse(sSettings[nSettingsIndex++]);

                //Motion deblur
                udMotionBlurLength.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udMotionBlurAngle.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                if (sSettings[nSettingsIndex++] == "y")
                    chkRotateImage.Checked = true;
                else
                    chkRotateImage.Checked = false;
                if (sSettings[nSettingsIndex++] == "y")
                    chkAntiAliasLine.Checked = true;
                else
                    chkAntiAliasLine.Checked = false;
                txtPSFLineThickness.Text = sSettings[nSettingsIndex++];

                //Field rotation
                if (sSettings[nSettingsIndex++] == "y")
                    chkFiledRotationDeblur.Checked = true;
                else
                    chkFiledRotationDeblur.Checked = false;
                udFieldRotationAngle.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udCentreFieldRotationX.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udCentreFieldRotationY.Value = Decimal.Parse(sSettings[nSettingsIndex++]);

                //Sharpening layers
                if (sSettings[nSettingsIndex++] == "y")
                    optSharpeningLayers.Checked = true;
                else
                    optSharpeningLayers.Checked = false;
                if (optSharpeningLayers.Checked)
                    mLoadLayerSettingsCombo(cboLayerSettings);

                mbIgnoreKernelSharpeningLayersChanged = true;
                cboKernelSharpeningLayers.SelectedIndex = int.Parse(sSettings[nSettingsIndex++]);
                mbIgnoreKernelSharpeningLayersChanged = false;

                if (sSettings[nSettingsIndex++] == "y")
                    chkLayers0.Checked = true;
                else
                    chkLayers0.Checked = false;
                trkLayers0.Value = int.Parse(sSettings[nSettingsIndex++]);

                if (sSettings[nSettingsIndex++] == "y")
                    chkLayers1.Checked = true;
                else
                    chkLayers1.Checked = false;
                trkLayers1.Value = int.Parse(sSettings[nSettingsIndex++]);

                if (sSettings[nSettingsIndex++] == "y")
                    chkLayers2.Checked = true;
                else
                    chkLayers2.Checked = false;
                trkLayers2.Value = int.Parse(sSettings[nSettingsIndex++]);

                if (sSettings[nSettingsIndex++] == "y")
                    chkLayers3.Checked = true;
                else
                    chkLayers3.Checked = false;
                trkLayers3.Value = int.Parse(sSettings[nSettingsIndex++]);

                if (sSettings[nSettingsIndex++] == "y")
                    chkLayers4.Checked = true;
                else
                    chkLayers4.Checked = false;
                trkLayers4.Value = int.Parse(sSettings[nSettingsIndex++]);

                if (sSettings[nSettingsIndex++] == "y")
                    chkLayers5.Checked = true;
                else
                    chkLayers5.Checked = false;
                trkLayers5.Value = int.Parse(sSettings[nSettingsIndex++]);

                udLayersScale.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udLayersNoiseControl.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
                udLayersImageScale.Value = Decimal.Parse(sSettings[nSettingsIndex++]);

                //Misc
                if (sSettings[nSettingsIndex++] == "y")
                    chkLaplacian.Checked = true;
                else
                    chkLaplacian.Checked = false;
                if (sSettings[nSettingsIndex++] == "y")
                    chkClickCompareTo.Checked = true;
                else
                    chkClickCompareTo.Checked = false;

                cboZoomMode.SelectedIndex = int.Parse(sSettings[nSettingsIndex++]);
            }
            mbSettingsLoaded = true;
        }

        private void mSaveSettings()
        {
            string sSettings = "";
            sSettings = sSettings + txtImage.Text + "|";
            if (chkAutostretch.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            if (chkAutoDeblur.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + udZoom.Value.ToString() + "|";
            if (optOriginal.Checked)
                sSettings = sSettings + "1" + "|";
            else
                sSettings = sSettings + "2" + "|";

            //Repair method
            sSettings = sSettings + trkBlur.Value.ToString() + "|";
            if (optWiener.Checked)
                sSettings = sSettings + "1" + "|";
            else if (optTikhonov.Checked)
                sSettings = sSettings + "2" + "|";
            else if (optLandweber.Checked)
                sSettings = sSettings + "3" + "|";
            else if (optLR.Checked)
                sSettings = sSettings + "4" + "|";
            else if (optRIF.Checked)
                sSettings = sSettings + "5" + "|";
            else 
                sSettings = sSettings + "6" + "|";
            sSettings = sSettings + txtNSR.Text + "|";
            sSettings = sSettings + txtCLS_Y.Text + "|";
            sSettings = sSettings + txtIterations.Text + "|";

            //PSF Display width
            sSettings = sSettings + udPSFPlotWidth.Value.ToString() + "|";

            //Edge repair
            if (chkRepairEdges.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            if (chkTanhRepair.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            if (chkRepairTopBottom.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + txtGamma.Text + "|";
            sSettings = sSettings + txtBeta.Text + "|";
            if (chkDeringing.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + udDeringStarThreshold.Value.ToString() + "|";
            sSettings = sSettings + udDeringRepairStrength.Value.ToString() + "|";

            //Feather
            sSettings = sSettings + txtFeather.Text + "|";

            //PSF Repair mode
            if (optCircularBlur.Checked)
                sSettings = sSettings + "1" + "|";
            else if (optMotionBlur.Checked)
                sSettings = sSettings + "2" + "|";
            else
                sSettings = sSettings + "3" + "|";

            //Circular PSF
            sSettings = sSettings + udFWHM.Value.ToString() + "|";
            sSettings = sSettings + udBrightness.Value.ToString() + "|";
            if (chkCropPSF.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + udCropPSF.Value.ToString() + "|";
            if (optVoigt.Checked)
                sSettings = sSettings + "1" + "|";
            else if (optMoffat.Checked)
                sSettings = sSettings + "2" + "|";
            else if (optGaussianDeblur.Checked)
                sSettings = sSettings + "3" + "|";
            else if (optCameraCircleDeblur.Checked)
                sSettings = sSettings + "4" + "|";
            else if (optMTFPSF.Checked)
                sSettings = sSettings + "5" + "|";
            sSettings = sSettings + udGaussFraction.Value.ToString() + "|";
            sSettings = sSettings + udMoffatBeta.Value.ToString() + "|";
            if (chkMoffatInPasses.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + udMoffatPasses.Value.ToString() + "|";
            sSettings = sSettings + udWave.Value.ToString() + "|";

            //Motion deblur
            sSettings = sSettings + udMotionBlurLength.Value.ToString() + "|";
            sSettings = sSettings + udMotionBlurAngle.Value.ToString() + "|";
            if (chkRotateImage.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            if (chkAntiAliasLine.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + txtPSFLineThickness.Text + "|";

            //Field rotation
            if (chkFiledRotationDeblur.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + udFieldRotationAngle.Value.ToString() + "|";
            sSettings = sSettings + udCentreFieldRotationX.Value.ToString() + "|";
            sSettings = sSettings + udCentreFieldRotationY.Value.ToString() + "|";

            //Sharpening layers
            if (optSharpeningLayers.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";

            sSettings = sSettings + cboKernelSharpeningLayers.SelectedIndex.ToString() + "|";  

            if (chkLayers0.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers0.Value.ToString() + "|";

            if (chkLayers1.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers1.Value.ToString() + "|";

            if (chkLayers2.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers2.Value.ToString() + "|";

            if (chkLayers3.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers3.Value.ToString() + "|";

            if (chkLayers4.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers4.Value.ToString() + "|";

            if (chkLayers5.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers5.Value.ToString() + "|";

            sSettings = sSettings + udLayersScale.Value.ToString() + "|";
            sSettings = sSettings + udLayersNoiseControl.Value.ToString() + "|";
            sSettings = sSettings + udLayersImageScale.Value.ToString() + "|";

            //Misc
            if (chkLaplacian.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            if (chkClickCompareTo.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";

            //Interpolation zoom mode
            sSettings = sSettings + cboZoomMode.SelectedIndex.ToString() + "|";

            File.WriteAllText("settings.txt", sSettings);
        }

        //Sharpening layers settings
        private bool mbSettingsComboLoaded = false;
        private void mLoadLayerSettingsCombo(ComboBox cbo)
        {
            cbo.Items.Clear();
            string[] sFiles = Directory.GetFiles(System.Environment.CurrentDirectory, "*.set");
            Array.Sort(sFiles);
            for (int i = 0; i < sFiles.Length; i++)
                cbo.Items.Add(Path.GetFileNameWithoutExtension(sFiles[i]));

            mbSettingsComboLoaded = true;
        }

        private void txtSaveSettingsName_TextChanged(object sender, EventArgs e)
        {
            if (txtSaveSettingsName.Text.Trim() == "")
                btnSaveLayersSettings.Enabled = false;
            else
                btnSaveLayersSettings.Enabled = true;
        }

        private void btnSaveLayersSettings_Click(object sender, EventArgs e)
        {
            string sSettingsFile = System.Environment.CurrentDirectory + @"\" + txtSaveSettingsName.Text + ".set";
            if (File.Exists(sSettingsFile))
                File.Delete(sSettingsFile);

            string sSettings = "";
            sSettings = sSettings + cboKernelSharpeningLayers.SelectedIndex.ToString() + "|";

            if (chkLayers0.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers0.Value.ToString() + "|";

            if (chkLayers1.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers1.Value.ToString() + "|";

            if (chkLayers2.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers2.Value.ToString() + "|";

            if (chkLayers3.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers3.Value.ToString() + "|";

            if (chkLayers4.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers4.Value.ToString() + "|";

            if (chkLayers5.Checked)
                sSettings = sSettings + "y" + "|";
            else
                sSettings = sSettings + "n" + "|";
            sSettings = sSettings + trkLayers5.Value.ToString() + "|";

            sSettings = sSettings + udLayersScale.Value.ToString() + "|";
            sSettings = sSettings + udLayersNoiseControl.Value.ToString() + "|";
            sSettings = sSettings + udLayersImageScale.Value.ToString() + "|";

            File.WriteAllText(sSettingsFile, sSettings);

            cboLayerSettings.Items.Add(txtSaveSettingsName.Text);
            cboLayerSettings.SelectedIndex = cboLayerSettings.Items.Count - 1;
        }

        private void cboLayerSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLayerSettings.SelectedIndex == -1)
                return;

            string sSettingsFile = System.Environment.CurrentDirectory + @"\"
                + cboLayerSettings.Items[cboLayerSettings.SelectedIndex].ToString() + ".set";

            if (!File.Exists(sSettingsFile))
                return;

            mbIgnoreLayersChanged = true;

            string[] sSettings = File.ReadAllText(sSettingsFile).Split(new char[] { '|' });
            int nSettingsIndex = 0;

            mbIgnoreKernelSharpeningLayersChanged = true;
            cboKernelSharpeningLayers.SelectedIndex = int.Parse(sSettings[nSettingsIndex++]);
            mbIgnoreKernelSharpeningLayersChanged = false;

            if (sSettings[nSettingsIndex++] == "y")
                chkLayers0.Checked = true;
            else
                chkLayers0.Checked = false;
            trkLayers0.Value = int.Parse(sSettings[nSettingsIndex++]);
            if (chkLayers0.Checked)
                mLastTrkLayersValue = trkLayers0.Value;

            if (sSettings[nSettingsIndex++] == "y")
                chkLayers1.Checked = true;
            else
                chkLayers1.Checked = false;
            trkLayers1.Value = int.Parse(sSettings[nSettingsIndex++]);
            if (chkLayers1.Checked)
                mLastTrkLayersValue = trkLayers1.Value;

            if (sSettings[nSettingsIndex++] == "y")
                chkLayers2.Checked = true;
            else
                chkLayers2.Checked = false;
            trkLayers2.Value = int.Parse(sSettings[nSettingsIndex++]);
            if (chkLayers2.Checked)
                mLastTrkLayersValue = trkLayers2.Value;

            if (sSettings[nSettingsIndex++] == "y")
                chkLayers3.Checked = true;
            else
                chkLayers3.Checked = false;
            trkLayers3.Value = int.Parse(sSettings[nSettingsIndex++]);
            if (chkLayers3.Checked)
                mLastTrkLayersValue = trkLayers3.Value;

            if (sSettings[nSettingsIndex++] == "y")
                chkLayers4.Checked = true;
            else
                chkLayers4.Checked = false;
            trkLayers4.Value = int.Parse(sSettings[nSettingsIndex++]);
            if (chkLayers4.Checked)
                mLastTrkLayersValue = trkLayers4.Value;

            if (sSettings[nSettingsIndex++] == "y")
                chkLayers5.Checked = true;
            else
                chkLayers5.Checked = false;
            trkLayers5.Value = int.Parse(sSettings[nSettingsIndex++]);
            if (chkLayers5.Checked)
                mLastTrkLayersValue = trkLayers5.Value;

            udLayersScale.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
            udLayersNoiseControl.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
            udLayersImageScale.Value = Decimal.Parse(sSettings[nSettingsIndex++]);
            mLastLayersScale = udLayersScale.Value;

            mbIgnoreLayersChanged = false;

            mbLayersCalc = false;
            mLayersChanged();
        }

        //Field rotation image functions


        //User interface EVENTS
        private Mat MercatorToNormal(Mat imgIn, int nOutWidth, int nOutHeight,
                float fPoleX, float fPoleY, int nOverlapPixels)
        {
            int nWidthExtended = imgIn.Width;
            int nWidth = nWidthExtended - nOverlapPixels * 2;
            int nHeight = imgIn.Height;
            float fWidth = nWidth;
            float fHeight = nHeight;
            int nChannels = imgIn.Channels();

            if (fPoleX == -1.0f)
                fPoleX = nOutWidth / 2.0f;
            if (fPoleY == -1.0f)
                fPoleY = nOutHeight / 2.0f;

            //Use arrays for input and output
            float[,] aImgIn = new float[nHeight, nWidthExtended];
            float[,] aOut = new float[nOutHeight, nOutWidth];
            float[,] aOutWgts = new float[nOutHeight, nOutWidth];

            float fVal = 0.0f;
            float fX = 0.0f;
            float fY = 0.0f;
            float fR = 0.0f;
            float fTh = 0.0f;
            float fCosTh = 0.0f;
            float fSinTh = 0.0f;
            int nX = 0;
            int nY = 0;
            float fFracX = 0.0f;
            float fFracY = 0.0f;
            float fWgt = 0.0f;

            Mat[] imgInFChannels = new Mat[0];
            Mat[] imgOutChannels = new Mat[0];
            if (nChannels > 1)
            {
                imgInFChannels = new Mat[nChannels];
                imgOutChannels = new Mat[nChannels];
                Cv2.Split(imgIn, out imgInFChannels);
            }

            for (int ch = 0; ch < nChannels; ch++)
            {
                if (nChannels == 1)
                    imgIn.GetArray(0, 0, aImgIn);
                else
                    imgInFChannels[ch].GetArray(0, 0, aImgIn);

                aOut = new float[nOutHeight, nOutWidth];
                aOutWgts = new float[nOutHeight, nOutWidth];

                //Go through input array
                for (int x = nOverlapPixels; x < nWidth + nOverlapPixels; x++)
                {
                    fTh = (float)(x - nOverlapPixels) / nWidth * 2.0f * (float)Math.PI;
                    fCosTh = (float)Math.Cos(fTh);
                    fSinTh = (float)Math.Sin(fTh);

                    for (int y = 0; y < nHeight; y++)
                    {
                        fR = (float)y;
                        fX = fR * fCosTh + fPoleX;
                        fY = fR * fSinTh + fPoleY;

                        fVal = aImgIn[y, x];

                        //Distribute this pixel to 4 nearest pixels in aOut
                        nX = (int)Math.Floor(fX);
                        nY = (int)Math.Floor(fY);
                        fFracX = fX - nX;
                        fFracY = fY - nY;

                        if (nX >= 0 && nY >= 0 && nX < nOutWidth && nY < nOutHeight)
                        {
                            fWgt = (1 - fFracX) * (1 - fFracY);
                            aOutWgts[nY, nX] += fWgt;
                            aOut[nY, nX] += fVal * fWgt;
                        }
                        if (nX + 1 >= 0 && nY >= 0 && nX + 1 < nOutWidth && nY < nOutHeight)
                        {
                            fWgt = fFracX * (1 - fFracY);
                            aOutWgts[nY, nX + 1] += fWgt;
                            aOut[nY, nX + 1] += fVal * fWgt;
                        }
                        if (nX >= 0 && nY + 1 >= 0 && nX < nOutWidth && nY + 1 < nOutHeight)
                        {
                            fWgt = (1 - fFracX) * fFracY;
                            aOutWgts[nY + 1, nX] += fWgt;
                            aOut[nY + 1, nX] += fVal * fWgt;
                        }
                        if (nX + 1 >= 0 && nY + 1 >= 0 && nX + 1 < nOutWidth && nY + 1 < nOutHeight)
                        {
                            fWgt = fFracX * fFracY;
                            aOutWgts[nY + 1, nX + 1] += fWgt;
                            aOut[nY + 1, nX + 1] += fVal * fWgt;
                        }
                    }
                }

                for (int x = 0; x < nOutWidth; x++)
                {
                    for (int y = 0; y < nOutHeight; y++)
                    {
                        if (aOutWgts[y, x] != 0.0f)
                            aOut[y, x] = aOut[y, x] / aOutWgts[y, x];
                    }
                }

                if (nChannels > 1)
                {
                    //Make copy of array, otherwise imgOutChannels[ch] changes as aOut changes for next channel
                    float[,] aOutCopy = new float[nOutWidth, nOutHeight];
                    Array.Copy(aOut, aOutCopy, aOut.Length);
                    imgOutChannels[ch] = new Mat(nOutHeight, nOutWidth, imgInFChannels[ch].Type(), aOutCopy);
                }
            }

            Mat repaired = null;
            if (nChannels == 1)
                repaired = new Mat(nOutHeight, nOutWidth, mImageTypeInternal, aOut);
            else
            {
                repaired = new Mat(nOutHeight, nOutWidth, mImageTypeInternal);
                Cv2.Merge(imgOutChannels, repaired);
            }

            return repaired;
        }

        private void MercatorSize(int nWidth, int nHeight, ref int nOutWidth, ref int nOutWidthExtended,
            ref int nOutHeight, float fPoleX, float fPoleY,
            float fOverlapPixels, float fOverlapDegrees, ref int nOverlapPixels)
        {
            //Maximum distance squared from pole
            float fMaxRadius2 = MaximumCornerDistanceFromPole_Squared(fPoleX, fPoleY, nWidth, nHeight);

            //Overlap before zero degrees and after 360 degrees
            if (fOverlapPixels == -1.0f)
            {
                if (fOverlapDegrees != 0.0f)
                    fOverlapPixels = (float)Math.Sqrt(fMaxRadius2) * fOverlapDegrees / 360.0f * (float)Math.PI;
            }
            nOverlapPixels = (int)fOverlapPixels;
            if (nOverlapPixels % 2 == 1)
                nOverlapPixels = nOverlapPixels + 1;

            //Do not create any overlap if not repairing edges
            if (!chkRepairEdges.Checked)
            {
                fOverlapPixels = 0.0f;
                nOverlapPixels = 0;
            }

            nOutHeight = (int)Math.Sqrt(fMaxRadius2);
            if (nOutHeight % 2 == 1)
                nOutHeight = nOutHeight + 1;
            nOutWidth = (int)(2.0f * Math.PI * Math.Sqrt(fMaxRadius2));
            nOutWidthExtended = nOutWidth + nOverlapPixels * 2; //Extend beyond 360 degrees (wrap)
        }

        private float MaximumCornerDistanceFromPole_Squared(float fPoleX, float fPoleY, float fWidth, float fHeight)
        {
            float fMaxRadius2 = fPoleX * fPoleX + fPoleY * fPoleY;
            if ((fWidth - fPoleX) * (fWidth - fPoleX) + fPoleY * fPoleY > fMaxRadius2)
                fMaxRadius2 = (fWidth - fPoleX) * (fWidth - fPoleX) + fPoleY * fPoleY;
            if (fPoleX * fPoleX + (fHeight - fPoleY) * (fHeight - fPoleY) > fMaxRadius2)
                fMaxRadius2 = fPoleX * fPoleX + (fHeight - fPoleY) * (fHeight - fPoleY);
            if ((fWidth - fPoleX) * (fWidth - fPoleX) + (fHeight - fPoleY) * (fHeight - fPoleY) > fMaxRadius2)
                fMaxRadius2 = (fWidth - fPoleX) * (fWidth - fPoleX) + (fHeight - fPoleY) * (fHeight - fPoleY);

            return fMaxRadius2;
        }

        private Mat ProjectToMercator(Mat imgIn, float fPoleX, float fPoleY,
            float fOverlapPixels, float fOverlapDegrees, ref int nOverlapPixels)
        {
            int nChannels = imgIn.Channels();

            int nWidth = imgIn.Width;
            int nHeight = imgIn.Height;
            float fWidth = nWidth;
            float fHeight = nHeight;

            if (fPoleX == -1.0f)
                fPoleX = fWidth / 2.0f;
            if (fPoleY == -1.0f)
                fPoleY = fHeight / 2.0f;

            int nOutHeight = 0;
            int nOutWidth = 0;
            int nOutWidthExtended = 0;
            MercatorSize(nWidth, nHeight, ref nOutWidth, ref nOutWidthExtended, ref nOutHeight,
                fPoleX, fPoleY, fOverlapPixels, fOverlapDegrees, ref nOverlapPixels);

            Mat mercator = new Mat(nOutHeight, nOutWidthExtended, imgIn.Type());

            //Use arrays for input and output
            float[,] aImgIn = new float[nHeight, nWidth];
            float[,] aOut = new float[nOutHeight, nOutWidthExtended];

            float fR = 0.0f;
            float fTh = 0.0f;
            float fCosTh = 0.0f;
            float fSinTh = 0.0f;
            float fX = 0.0f;
            float fY = 0.0f;
            int nX = 0;
            int nY = 0;
            float f00 = 0.0f;
            float f01 = 0.0f;
            float f10 = 0.0f;
            float f11 = 0.0f;
            float fFracX = 0.0f;
            float fFracY = 0.0f;

            Mat[] imgInFChannels = new Mat[0];
            Mat[] imgOutChannels = new Mat[0];
            if (nChannels > 1)
            {
                imgInFChannels = new Mat[nChannels];
                imgOutChannels = new Mat[nChannels];
                Cv2.Split(imgIn, out imgInFChannels);
            }

            for (int ch = 0; ch < nChannels; ch++)
            {
                if (nChannels == 1)
                    imgIn.GetArray(0, 0, aImgIn);
                else
                    imgInFChannels[ch].GetArray(0, 0, aImgIn);

                for (int x = 0; x < nOutWidthExtended; x++)
                {
                    fTh = (float)(x - nOverlapPixels) / nOutWidth * 2.0f * (float)Math.PI;
                    fCosTh = (float)Math.Cos(fTh);
                    fSinTh = (float)Math.Sin(fTh);

                    for (int y = 0; y < nOutHeight; y++)
                    {
                        fR = (float)y;
                        fX = fR * fCosTh + fPoleX;
                        fY = fR * fSinTh + fPoleY;
                        nX = (int)Math.Floor(fX);
                        nY = (int)Math.Floor(fY);
                        fFracX = fX - nX;
                        fFracY = fY - nY;

                        //Find value in imgIn at this (r,theta) location
                        if (nX < 0 || nY < 0 || nX + 1 >= nWidth || nY + 1 >= nHeight)
                            aOut[y, x] = -1.0f;
                        else
                        {
                            f00 = aImgIn[nY, nX];
                            f10 = aImgIn[nY + 1, nX];
                            f01 = aImgIn[nY, nX + 1];
                            f11 = aImgIn[nY + 1, nX + 1];

                            if (f00 == 0.0f || f01 == 0.0f || f10 == 0.0f || f11 == 0.0f) //Zero edge borders, make the output value be undefined (=-1)
                                aOut[y, x] = -1.0f;
                            else
                                aOut[y, x] = f00 * (1 - fFracX) * (1 - fFracY) + f10 * fFracY * (1 - fFracX) +
                                            f01 * (1 - fFracY) * fFracX + f11 * fFracX * fFracY;
                        }
                    }
                }

                //Fill in missing areas of rectangle where aOut[y,x] == -1.0f
                float fLastNonZeroVal = -1.0f;
                int nLastNonZeroPos = -1;
                float fNextNonZeroVal = -1.0f;
                int nNextNonZeroPos = -1;
                for (int y = 0; y < nOutHeight; y++)
                {
                    nLastNonZeroPos = -1;
                    nNextNonZeroPos = -1;
                    fLastNonZeroVal = -1.0f;
                    fNextNonZeroVal = -1.0f;

                    for (int x = 0; x < nOutWidth + nOverlapPixels * 2; x++)
                    {
                        if (aOut[y, x] != -1.0f)
                        {
                            //Store current vals
                            fLastNonZeroVal = aOut[y, x];
                            nLastNonZeroPos = x;
                            if (nNextNonZeroPos != -1)
                            {
                                //Reset
                                nNextNonZeroPos = -1;
                                fNextNonZeroVal = -1.0f;
                            }
                        }
                        else
                        {
                            //Current pixel is empty, fill with interpolated between last and next non-empty pixels
                            if (nLastNonZeroPos == -1)
                            {
                                for (int nPrevX = x; nPrevX >= nOverlapPixels; nPrevX--)
                                {
                                    if (aOut[y, nPrevX] != -1.0f)
                                    {
                                        fLastNonZeroVal = aOut[y, nPrevX];
                                        nLastNonZeroPos = nPrevX;
                                        break;
                                    }
                                }
                            }
                            //Start from other side if still not found last value
                            if (nLastNonZeroPos == -1)
                            {
                                for (int nPrevX = nOutWidth + nOverlapPixels - 1; nPrevX >= x; nPrevX--)
                                {
                                    if (aOut[y, nPrevX] != -1.0f)
                                    {
                                        fLastNonZeroVal = aOut[y, nPrevX];
                                        nLastNonZeroPos = nPrevX;
                                        break;
                                    }
                                }
                            }

                            if (nNextNonZeroPos == -1)
                            {
                                for (int nNextX = x; nNextX < nOutWidth + nOverlapPixels; nNextX++)
                                {
                                    if (aOut[y, nNextX] != -1.0f)
                                    {
                                        fNextNonZeroVal = aOut[y, nNextX];
                                        nNextNonZeroPos = nNextX;
                                        break;
                                    }
                                }
                            }
                            //Start from other side if still not found next value
                            if (nNextNonZeroPos == -1)
                            {
                                for (int nNextX = nOverlapPixels; nNextX < x + nOverlapPixels; nNextX++)
                                {
                                    if (aOut[y, nNextX] != -1.0f)
                                    {
                                        fNextNonZeroVal = aOut[y, nNextX];
                                        nNextNonZeroPos = nNextX;
                                        break;
                                    }
                                }
                            }

                            float fDistanceBetweenLastAndNext = nNextNonZeroPos - nLastNonZeroPos;
                            if (fDistanceBetweenLastAndNext < 0.0f)
                                fDistanceBetweenLastAndNext = fDistanceBetweenLastAndNext + nOutWidth;

                            float fDistanceFromLastNonZero = x - nLastNonZeroPos;
                            if (fDistanceFromLastNonZero < 0.0f)
                                fDistanceFromLastNonZero = fDistanceFromLastNonZero + nOutWidth;

                            float fFrac = fDistanceFromLastNonZero / fDistanceBetweenLastAndNext;

                            aOut[y, x] = fLastNonZeroVal * (1.0f - fFrac) + fNextNonZeroVal * fFrac;
                        }
                    }
                }

                if (nChannels > 1)
                {
                    //Make copy of array, otherwise imgOutChannels[ch] changes as aOut changes for next channel
                    float[,] aOutCopy = new float[nOutWidthExtended, nOutHeight];
                    Array.Copy(aOut, aOutCopy, aOut.Length);
                    imgOutChannels[ch] = new Mat(nOutHeight, nOutWidthExtended, imgInFChannels[ch].Type(), aOutCopy);
                }
            }

            if (nChannels == 1)
                mercator = new Mat(nOutHeight, nOutWidthExtended, imgIn.Type(), aOut);
            else
                Cv2.Merge(imgOutChannels, mercator);

            return mercator;
        }

        private void btnFileLocation_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Image Files ...",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "",
                Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF,*.TIF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF;*.TIF|" + "All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtImage.Text = openFileDialog1.FileName;
                this.Cursor = Cursors.WaitCursor;
                ImageLoad();
                this.Cursor = Cursors.Default;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            mSaveSettings();
        }

        private void mIterationsValueChanged()
        {
            if (!mbMatIterationsStored)
                return;
            if (mbIgnoreIterationsChanged)
                return;
            this.Cursor = Cursors.WaitCursor;

            //Final output into Mat
            int nTrkVal = trkIterations.Value;
            int nTargetIterationNumber = (int)((float)nTrkVal / trkIterations.Maximum * mnNoOfIterationsStored);
            if (nTargetIterationNumber >= mDeconvolvedMatIterations[0].Length)
                nTargetIterationNumber = mDeconvolvedMatIterations[0].Length - 1;

            Mat[] output = new Mat[mnChannels];
            for (int nChannel = 0; nChannel < mnChannels; nChannel++)
            {
                output[nChannel] = mDeconvolvedMatIterations[nChannel][nTargetIterationNumber];
            }

            Mat imgTifOut = CombineChannels(mnWidth, mnHeight, mnChannels, output, false);

            if (optMotionBlur.Checked && chkFiledRotationDeblur.Checked)
                //Project from Mercator to normal
                imgTifOut = MercatorToNormal(imgTifOut, mnWidth, mnHeight,
                     mfFieldRotationCentreX, mfFieldRotationCentreY, mnOverlapPixels);

            mREPAIRED = imgTifOut.Clone();

            if (mbROISet)
            {
                //Copy repaired mREPAIRED over the top of imgToDeblur_Input
                Mat mSmall = mREPAIRED.Clone();
                mREPAIRED = imgToDeblur_Input;
                Rect roi = new Rect(mnROIImageStartX, mnROIImageStartY, mnROIImageWidth, mnROIImageHeight);
                mSmall.CopyTo(new Mat(mREPAIRED, roi));
                mSmall.Dispose();
            }

            if (chkAutostretch.Checked)
                mREPAIRED = StretchImagePercentiles(mREPAIRED, mnPercentilesStretchMin, mnPercentilesStretchMax);

            MatToPictureBox_Zoomed(mREPAIRED, picOut, "", true, false, false);

            this.Cursor = Cursors.Default;
        }

        private void btnStopIterations_Click(object sender, EventArgs e)
        {
            mbProcessingCancelled = true;
        }

        private void trkBlur_ValueChanged(object sender, EventArgs e)
        {
            //Change NSR for Wiener and Tikhinov
            double dBlur = (double)trkBlur.Value;
            txtNSR.Text =  (Math.Pow(1.122, dBlur) / 100000.0).ToString("0.00000"); //Range 1e-5 to 1
            txtCLS_Y.Text = (Math.Pow(1.122, dBlur) / 100000.0 / 1.5).ToString("0.00000"); //Range 1e-5 to 1 (/1.5)
        }

        private void udMoffatBeta_ValueChanged(object sender, EventArgs e)
        {
            if (udMoffatBeta.Value > 2.0m)
                udMoffatBeta.Increment = 0.5m;
            else
                udMoffatBeta.Increment = 0.1m;
            UIChangedDoAutoDeblur();
        }

        private void optMTFPSF_CheckedChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void udWave_ValueChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void udFWHM_ValueChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void optWiener_CheckedChanged(object sender, EventArgs e)
        {
            if (optWiener.Checked)
                UIChangedDoAutoDeblur();
        }

        private void optRIF_CheckedChanged(object sender, EventArgs e)
        {
            if (optRIF.Checked)
                UIChangedDoAutoDeblur();
        }

        private void optTikhonov_CheckedChanged(object sender, EventArgs e)
        {
            if (optTikhonov.Checked)
                UIChangedDoAutoDeblur();
        }

        private void udMotionBlurLength_ValueChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void udMotionBlurAngle_ValueChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void txtFeather_TextChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void trkBlur_Scroll(object sender, EventArgs e)
        {
            tip.SetToolTip((TrackBar)sender, ((TrackBar)sender).Value.ToString());
        }

        private void trkIterations_Scroll(object sender, EventArgs e)
        {
            tip.SetToolTip((TrackBar)sender, (((TrackBar)sender).Value * mnIterationUIFactor).ToString());
        }

        private void trkBlur_MouseUp(object sender, MouseEventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void trkBlur_KeyUp(object sender, KeyEventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void txtBlurLength_TextChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void txtAngle_TextChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void udBrightness_ValueChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void udCropPSF_ValueChanged(object sender, EventArgs e)
        {
            if (udCropPSF.Value < 2.0m)
                udCropPSF.Increment = 0.2m;
            else if (udCropPSF.Value < 5.0m)
                udCropPSF.Increment = 0.5m;
            else if (udCropPSF.Value < 12.0m)
                udCropPSF.Increment = 2.0m;
            else
                udCropPSF.Increment = 5.0m;
            UIChangedDoAutoDeblur();
        }

        private void chkCropPSF_CheckedChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void udGaussFraction_ValueChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void udLorentzRadius_ValueChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void UIChangedDoAutoDeblur()
        {
            if (!mbImageInLoaded)
                return;
            if (!optLandweber.Checked && !optLR.Checked && chkAutoDeblur.Checked)
            {
                Control curControl = this.ActiveControl;
                if (optCircularBlur.Checked)
                {
                    grpPSF.Enabled = false;
                    grpMethod.Enabled = false;
                }
                if (optMotionBlur.Checked)
                {
                    grpMotionBlurDetails.Enabled = false;
                    grpMethod.Enabled = false;
                }
                btnDeblur_Click(null, null);
                if (optCircularBlur.Checked)
                {
                    grpPSF.Enabled = true;
                    grpMethod.Enabled = true;
                }
                if (optMotionBlur.Checked)
                {
                    grpMotionBlurDetails.Enabled = true;
                    grpMethod.Enabled = true;
                }
                this.ActiveControl = curControl;
            }
        }

        private void btnCopyPSF_Click(object sender, EventArgs e)
        {
            txtDebug.SelectAll();
            txtDebug.Copy();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string sImageIn = txtImage.Text;
            if (sImageIn.StartsWith(@"\"))
                sImageIn = Environment.CurrentDirectory + sImageIn;
            else if ( sImageIn.IndexOf(@"\") == -1)
                sImageIn = Environment.CurrentDirectory + @"\" + sImageIn;

            string sExistingFileName = Path.GetFileNameWithoutExtension(sImageIn);
            string sOutputDir = Path.GetDirectoryName(sImageIn);
            string sNewName = "";
            Mat mSave = null;
            if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
            {
                sNewName = msRepairedDesription.Replace(":", "_");
                mSave = mREPAIRED;
            }
            else
            {
                sNewName = cboCurrent.SelectedItem.ToString().Replace(":", "_");
                mSave = mHistory[mHistoryCurrent];
            }

            if (sNewName.Length > 230 - sExistingFileName.Length)
            sNewName = sNewName.Substring(0, 230 - sExistingFileName.Length);
            string sOutputFile = sOutputDir + @"\" + sExistingFileName + "_" + sNewName + ".repaired.tif";
            //Cv2.ImWrite(sOutputFile, mSave);
            sOutputFile = sOutputFile.Replace(">", "");
            SaveImage(mSave, sOutputFile);
            this.Cursor = Cursors.Default;
        }

        private void btnShowLayers_Click(object sender, EventArgs e)
        {
            if (!mbImageInLoaded || !mbSettingsLoaded)
                return;

            if (mREPAIRED == null)
                return;

            this.Cursor = Cursors.WaitCursor;

            int nOrigWidth = mREPAIRED.Width;
            int nOrigHeight = mREPAIRED.Height;

            //Fit 2 small images along width
            int nLayerWidth = nOrigWidth / 2;
            //Preserve aspect ratio
            int nLayerHeight = (int)Math.Round((float)nLayerWidth * nOrigHeight / nOrigWidth, 0);
            //And 3 images down
            if (nLayerHeight * 3 > nOrigHeight)
            {
                nLayerHeight = nOrigHeight / 3;
                nLayerWidth = (int)Math.Round((float)nLayerHeight * nOrigWidth / nOrigHeight, 0);
            }
            Mat imgLayerSmall = new Mat(nLayerHeight, nLayerWidth, mREPAIRED.Type(), new Scalar(0.0f));
            mREPAIRED = new Mat(nOrigHeight, nOrigWidth, mREPAIRED.Type(), new Scalar(0.5f, 0.5f, 0.5f));
            Rect roi = new Rect();
            int nRectStartX = 0;
            int nRectStartY = 0;
            Mat mLayerAddHalf = new Mat();
            Mat[] mChannels = new Mat[mnChannels];
            float fLayerPowerScale = 1.0f;
            
            for (int i = 0; i < mLAYERS.Length; i++)
            {
                mLayerAddHalf = mLAYERS[i].Clone();
                fLayerPowerScale = (float)udLayersScale.Value;
                if (mnChannels == 1)
                    mLayerAddHalf = mLayerAddHalf * fLayerPowerScale + 0.5f;
                else
                {
                    mChannels = Cv2.Split(mLayerAddHalf);
                    for (int nCh = 0; nCh < mnChannels; nCh++)
                    {
                        mChannels[nCh] = mChannels[nCh] * fLayerPowerScale + 0.5f;
                    }
                    Cv2.Merge(mChannels, mLayerAddHalf);
                }
                Cv2.Resize(mLayerAddHalf, imgLayerSmall, new Size(nLayerWidth, nLayerHeight), 0.0f, 0.0f, InterpolationFlags.Lanczos4);
                if (i % 2 == 0)
                    nRectStartX = 0;
                else
                    nRectStartX = nLayerWidth ;
                nRectStartY = i / 2 * nLayerHeight;
                roi = new Rect(nRectStartX, nRectStartY, nLayerWidth, nLayerHeight);
                imgLayerSmall.CopyTo(new Mat(mREPAIRED, roi));
            }

            mLayerAddHalf.Dispose();
            imgLayerSmall.Dispose();
            if (mnChannels > 1)
            {
                for (int nCh = 0; nCh < mnChannels; nCh++)
                {
                    mChannels[nCh].Dispose();
                }
            }

            if (cboCurrent.Items[cboCurrent.Items.Count - 1].ToString() != "REPAIRED")
            cboCurrent.Items.Add("REPAIRED");

            msRepairedDesription = "Sharpening layers x 6";
            if (cboCurrent.SelectedIndex != cboCurrent.Items.Count - 1)
            {
                mbIgnoreCurrentDisplayCboChange = true;
                cboCurrent.SelectedIndex = cboCurrent.Items.Count - 1;
                mbIgnoreCurrentDisplayCboChange = false;
            }
            MatToPictureBox_Zoomed(mREPAIRED, picOut, "", true, false, false);

            this.Cursor = Cursors.Default;
        }

        private void btnDetailMask_Click(object sender, EventArgs e)
        {
            if (!mbImageInLoaded || !mbSettingsLoaded)
                return;

            if (mREPAIRED == null)
                return;

            this.Cursor = Cursors.WaitCursor;

            //Display mask for detail enhancement
            mREPAIRED = mREPAIRED.Laplacian(mREPAIRED.Depth());
            float fSigma = (float)udFWHM.Value / 2.35f / 0.707f;
            Cv2.GaussianBlur(mREPAIRED, mREPAIRED, new OpenCvSharp.Size(0, 0), fSigma);
            mREPAIRED = Cv2.Abs(mREPAIRED);
            mREPAIRED = (mREPAIRED - 0.00f) * 20.0f;
            Cv2.Dilate(mREPAIRED, mREPAIRED, new Mat(3, 3, MatType.CV_8U), new Point(-1, -1), 1);
            Cv2.GaussianBlur(mREPAIRED, mREPAIRED, new OpenCvSharp.Size(0, 0), fSigma);

            if (cboCurrent.Items[cboCurrent.Items.Count - 1].ToString() != "REPAIRED")
                cboCurrent.Items.Add("REPAIRED");

            msRepairedDesription = "Detail mask";
            if (cboCurrent.SelectedIndex != cboCurrent.Items.Count - 1)
            {
                mbIgnoreCurrentDisplayCboChange = true;
                cboCurrent.SelectedIndex = cboCurrent.Items.Count - 1;
                mbIgnoreCurrentDisplayCboChange = false;
            }
            MatToPictureBox_Zoomed(mREPAIRED, picOut, "", true, false, false);

            this.Cursor = Cursors.Default;
        }

        private void udPSFPlotWidth_ValueChanged(object sender, EventArgs e)
        {
            //Update picPSFProfile and picPSF
            if (!mbSettingsLoaded || !mbImageInLoaded)
                return;

            cboCurrent_SelectedIndexChanged(null, null);
        }

        private void cboCurrent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mbIgnoreCurrentDisplayCboChange)
                return;
            Mat mCurrent = null;
            Mat mCurrentPSF = null;
            Mat mCurrentFTDisplayed = null;
            string sDescription = "";
            float fPSFHistoryFWHM = 0.0f;
            if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
            {
                mCurrent = mREPAIRED;
                sDescription = msRepairedDesription;
                if (chkInvPSF.Checked)
                    mCurrentPSF = mPsfInvREPAIRED;
                else
                    mCurrentPSF = mPsfREPAIRED;
                mCurrentFTDisplayed = mFTDisplayREPAIRED;
                fPSFHistoryFWHM = mFWHMREPAIRED;
            }
            else
            {
                mCurrent = mHistory[cboCurrent.SelectedIndex];
                sDescription = cboCurrent.Items[cboCurrent.SelectedIndex].ToString();
                if (chkInvPSF.Checked)
                    mCurrentPSF = mPSFInvHistory[cboCurrent.SelectedIndex];
                else
                    mCurrentPSF = mPSFHistory[cboCurrent.SelectedIndex];
                mCurrentFTDisplayed = mFTDisplayHistory[cboCurrent.SelectedIndex];
                fPSFHistoryFWHM = mPSFHistoryFWHM[cboCurrent.SelectedIndex];
                mHistoryCurrent = cboCurrent.SelectedIndex;
            }

            if (sDescription.ToLower().IndexOf("motion") == -1 && sDescription.ToLower().IndexOf("rotation") == -1)
            {
                PlotPSFProfile(mCurrentPSF, picPSFProfile, (int)udPSFPlotWidth.Value, fPSFHistoryFWHM);
                PlotMTF(mPSFHistory[cboCurrent.SelectedIndex], fPSFHistoryFWHM);
            }
            else
            {
                picPSFProfile.Image = null;
            }

            if (optFilterFourierTransform.Checked)
                mDisplayFFTInPic(mCurrentFTDisplayed, picFilterFT, true, 0.0d, (double)udMaxPlotFT.Value);


            MatToPictureBox_Zoomed(mCurrent, picOut, sDescription, true, false, false);
            Mat psfCropped = PSFCroppedForDisplay(mCurrentPSF, (int)(udPSFPlotWidth.Value), 65000.0f);
            MatToPictureBox(psfCropped, picPSF, false, new System.Drawing.Point());
            if (psfCropped != null)
                psfCropped.Dispose();
        }

        int mnMouseDownX = 0;
        int mnMouseDownY = 0;
        int mnXContextClick = 0;
        int mnYContextClick = 0;
        int mnROIStartX = 0;
        int mnROIStartY = 0;
        int mnROIImageStartX = 0;
        int mnROIImageStartY = 0;
        int mnROIImageWidth = 0;
        int mnROIImageHeight = 0;
        bool mbMouseDown = false;
        private void picOut_MouseDown(object sender, MouseEventArgs e)
        {
            if (!mbImageInLoaded)
                return;
            if (!mbROISetPending)
                this.Cursor = Cursors.Hand;

            btnDeblur.Focus();
            mbMouseDown = true;
            mnMouseDownX = e.X;
            mnMouseDownY = e.Y;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                mnXContextClick = pnlOut.Left + picOut.Left + e.X;
                mnYContextClick = pnlOut.Top + picOut.Top + e.Y;
                mnuRightMain.Show(this, new System.Drawing.Point(mnXContextClick, mnYContextClick));
            }
            else
            {
                if (mbROISetPending)
                {
                    mnROIStartX =  e.X;
                    mnROIStartY =  e.Y;
                }
                else if (chkClickCompareTo.Checked && msRepairedDesription != "Sharpening layers x 6")
                {
                    //Swap displayed image
                    Mat mLast = mHistory[cboHistory.SelectedIndex];
                    string sDescription = cboHistory.Items[cboHistory.SelectedIndex].ToString();
                    MatToPictureBox_Zoomed(mLast, picOut, sDescription, true, false, false);

                    Mat mCurrentPSF = new Mat();
                    if (chkInvPSF.Checked)
                        mCurrentPSF = mPSFInvHistory[cboHistory.SelectedIndex];
                    else
                        mCurrentPSF = mPSFHistory[cboHistory.SelectedIndex];

                    if (sDescription.ToLower().IndexOf("motion") == -1 && sDescription.ToLower().IndexOf("rotation") == -1)
                    {
                        PlotPSFProfile(mCurrentPSF, picPSFProfile, (int)udPSFPlotWidth.Value, 0.0f);
                        PlotMTF(mPSFHistory[cboHistory.SelectedIndex], mPSFHistoryFWHM[cboHistory.SelectedIndex]);
                    }
                    else
                    {
                        picPSFProfile.Image = null;
                    }

                    if (optFilterFourierTransform.Checked)
                        mDisplayFFTInPic( mFTDisplayHistory[cboHistory.SelectedIndex], picFilterFT, true, 0.0d, (double)udMaxPlotFT.Value);

                    Mat psfCropped = PSFCroppedForDisplay(mCurrentPSF, (int)(udPSFPlotWidth.Value), 65000.0f);
                    MatToPictureBox(psfCropped, picPSF, false, new System.Drawing.Point());
                }
            }
        }

        private bool mbROISetPending = false;
        private bool mbROISet = false;
        private System.Drawing.Image mimgCurrent = null; 
        private void mnuSetArea_Click(object sender, EventArgs e)
        {
            mbROISetPending = true;
            //mimgCurrent needs to be Format24bppRgb to get graphics object to draw rectangle
            if (picOut.Image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                System.Drawing.Bitmap bmpArea = new System.Drawing.Bitmap(picOut.Image.Width, picOut.Image.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpArea);
                g.DrawImage(picOut.Image, 0, 0);
                mimgCurrent = bmpArea;
            }
            else
                mimgCurrent = picOut.Image;
            this.Cursor = Cursors.SizeNWSE;
        }

        private void mnuSetWholeImage_Click(object sender, EventArgs e)
        {
            mbROISetPending = false;
            mbROISet = false;
            mbLayersCalc = false;

            if (optCircularBlur.Checked || optMotionBlur.Checked)
            {
                UIChangedDoAutoDeblur();
            }

            if (optSharpeningLayers.Checked)
            {
                mSetUpSharpeningLayers();
                mLayersChanged();
            }
        }

        private void picOut_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mbImageInLoaded)
                return;
            if (cboCurrent.SelectedItem == null)
                return;
            if (mbMouseDown)
            {
                if (mbROISetPending)
                {
                    int nRectEndX =  e.X;
                    int nRectEndY =  e.Y;
                    int nRectWidth = nRectEndX -mnROIStartX ;
                    int nRectHeight = nRectEndY - mnROIStartY ;

                    System.Drawing.Image imgNew = (System.Drawing.Image)mimgCurrent.Clone();
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(imgNew);
                    g.DrawRectangle(System.Drawing.Pens.White, mnROIStartX, mnROIStartY, nRectWidth, nRectHeight);
                    picOut.Image = imgNew;
                }
                else
                {
                    int nMoveX = e.X - mnMouseDownX;
                    int nMoveY = e.Y - mnMouseDownY;
                    picOut.Location = new System.Drawing.Point(picOut.Location.X + nMoveX, picOut.Location.Y + nMoveY);
                }
            }
            else
            {
                Decimal fImageX = e.X / mdScale + mnZoomedImageTopLeftX;
                Decimal fImageY = e.Y / mdScale + mnZoomedImageTopLeftY;
                Mat mDisplayed = new Mat();
                if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
                    mDisplayed = mREPAIRED;
                else
                    mDisplayed = mHistory[cboCurrent.SelectedIndex];
                if (mDisplayed == null)
                    return;
                float fImgPx = mDisplayed.At<float>((int)Math.Floor(fImageY), (int)Math.Floor(fImageX));
                lblPixelPos.Text = fImageX.ToString("0.0") + "," + fImageY.ToString("0.0") + "\r\n" +
                    (fImgPx*mnImageDepth).ToString("0");
            }
        }

        private void picOut_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mbImageInLoaded)
                return;
            this.Cursor = Cursors.Default;
            mbMouseDown = false;

            Decimal nMouseZoomAtPanelX = e.X - pnlOut.Location.X;
            Decimal nMouseZoomAtPanelY = e.Y - pnlOut.Location.Y;

            Mat mCurrent = null;
            Mat mCurrentPSF = null;
            Mat mCurrentFTDisplayed = null;
            string sDescription = "";

            if (mbROISetPending)
            {
                int nRectEndX = e.X;
                int nRectEndY = e.Y;
                int nRectWidth = nRectEndX - mnROIStartX;
                int nRectHeight = nRectEndY - mnROIStartY;

                mbROISetPending = false;
                Decimal fImageX = mnROIStartX / mdScale + mnZoomedImageTopLeftX;
                Decimal fImageY = mnROIStartY / mdScale + mnZoomedImageTopLeftY;
                mnROIImageStartX = (int)Math.Round(fImageX, 0);
                mnROIImageStartY = (int)Math.Round(fImageY, 0);

                fImageX = e.X / mdScale + mnZoomedImageTopLeftX;
                fImageY = e.Y / mdScale + mnZoomedImageTopLeftY;
                mnROIImageWidth = (int)Math.Round(fImageX, 0) - mnROIImageStartX;
                mnROIImageHeight = (int)Math.Round(fImageY, 0) - mnROIImageStartY;
                if (mnROIImageStartX + mnROIImageWidth >= mnWidth)
                    mnROIImageWidth = mnWidth - mnROIImageStartX - 1;
                if (mnROIImageStartY + mnROIImageHeight >= mnHeight)
                    mnROIImageHeight = mnHeight - mnROIImageStartY - 1;
                //Make dimensions be even
                if (mnROIImageWidth % 2 == 1)
                    mnROIImageWidth = mnROIImageWidth - 1;
                if (mnROIImageHeight % 2 == 1)
                    mnROIImageHeight = mnROIImageHeight - 1;

                if (mnROIImageWidth <= 0 || mnROIImageHeight <= 0)
                    mbROISet = false;
                else
                {
                    mbROISet = true;
                    if (optCircularBlur.Checked || optMotionBlur.Checked)
                        UIChangedDoAutoDeblur();
                    if (optSharpeningLayers.Checked)
                    {
                        mbLayersCalc = false;
                        mSetUpSharpeningLayers();
                        mLayersChanged();
                    }
                }

                mimgCurrent.Dispose();
            }
            else
            {
                if (!chkClickCompareTo.Checked)
                {
                    if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
                    {
                        mCurrent = mREPAIRED;
                        sDescription = msRepairedDesription;
                        if (chkInvPSF.Checked)
                            mCurrentPSF = mPsfInvREPAIRED;
                        else
                            mCurrentPSF = mPsfREPAIRED;
                        mCurrentFTDisplayed = mFTDisplayREPAIRED;
                    }
                    else
                    {
                        mCurrent = mHistory[cboCurrent.SelectedIndex];
                        sDescription = cboCurrent.Items[cboCurrent.SelectedIndex].ToString();
                        if (chkInvPSF.Checked)
                            mCurrentPSF = mPSFInvHistory[cboCurrent.SelectedIndex];
                        else
                            mCurrentPSF = mPSFHistory[cboCurrent.SelectedIndex];
                        mCurrentFTDisplayed = mFTDisplayHistory[cboCurrent.SelectedIndex];
                    }
                }
                else
                {
                    float fPSFHistoryFWHM = 0.0f;

                    if (optSharpeningLayers.Checked)
                    {
                        sDescription = msRepairedDesription;
                        mCurrent = mREPAIRED;
                        if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
                        {
                            mCurrentFTDisplayed = mFTDisplayREPAIRED;
                            if (chkInvPSF.Checked)
                                mCurrentPSF = mPsfInvREPAIRED;
                            else
                                mCurrentPSF = mPsfREPAIRED;
                        }
                        else
                        {
                            mCurrentFTDisplayed = mFTDisplayHistory[cboCurrent.SelectedIndex];
                            if (chkInvPSF.Checked)
                                mCurrentPSF = mPSFInvHistory[cboCurrent.SelectedIndex];
                            else
                                mCurrentPSF = mPSFHistory[cboCurrent.SelectedIndex];
                        }
                    }
                    else
                    {
                        if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
                        {
                            mCurrent = mREPAIRED;
                            sDescription = msRepairedDesription;
                            if (chkInvPSF.Checked)
                                mCurrentPSF = mPsfInvREPAIRED;
                            else
                                mCurrentPSF = mPsfREPAIRED;
                            mCurrentFTDisplayed = mFTDisplayREPAIRED;
                            fPSFHistoryFWHM = mFWHMREPAIRED;
                        }
                        else
                        {
                            mCurrent = mHistory[cboCurrent.SelectedIndex];
                            sDescription = cboCurrent.Items[cboCurrent.SelectedIndex].ToString();
                            if (chkInvPSF.Checked)
                                mCurrentPSF = mPSFInvHistory[cboCurrent.SelectedIndex];
                            else
                                mCurrentPSF = mPSFHistory[cboCurrent.SelectedIndex];
                            mCurrentFTDisplayed = mFTDisplayHistory[cboCurrent.SelectedIndex];
                            fPSFHistoryFWHM = mPSFHistoryFWHM[cboCurrent.SelectedIndex];
                        }
                    }

                    //Display PSF details
                    if (sDescription.ToLower().IndexOf("motion") == -1 && sDescription.ToLower().IndexOf("rotation") == -1)
                    {
                        PlotPSFProfile(mCurrentPSF, picPSFProfile, (int)udPSFPlotWidth.Value, fPSFHistoryFWHM);
                        PlotMTF(mPSFHistory[cboCurrent.SelectedIndex], fPSFHistoryFWHM);
                    }
                    else
                    {
                        picPSFProfile.Image = null;
                    }

                    if (optFilterFourierTransform.Checked)
                        mDisplayFFTInPic( mCurrentFTDisplayed, picFilterFT, true, 0.0d, (double)udMaxPlotFT.Value);

                    Mat psfCropped = PSFCroppedForDisplay(mCurrentPSF, (int)(udPSFPlotWidth.Value), 65000.0f);
                    MatToPictureBox(psfCropped, picPSF, false, new System.Drawing.Point(0, 0));
                }

                bool bRecentreImage = false;
                if (mnZoomedImageWidth != 0)
                {
                    //Check if we have panned so that the picture box is no longer completly covered with the cropped and zoomed image
                    Decimal[] fImageX = new decimal[2];
                    Decimal[] fImageY = new decimal[2];
                    PicBoxXY_ToImagePixelXY((PictureBox)sender, mdScale, 0, 0, ref fImageX[0], ref fImageY[0], mnZoomedImageTopLeftX, mnZoomedImageTopLeftY);
                    PicBoxXY_ToImagePixelXY((PictureBox)sender, mdScale, pnlOut.Width, pnlOut.Height,
                        ref fImageX[1], ref fImageY[1], mnZoomedImageTopLeftX, mnZoomedImageTopLeftY);

                    if (fImageX[0] < mnZoomedImageTopLeftX || fImageY[0] < mnZoomedImageTopLeftY ||
                            fImageX[1] > mnZoomedImageTopLeftX + mnZoomedImageWidth ||
                            fImageY[1] > mnZoomedImageTopLeftY + mnZoomedImageHeight)
                        bRecentreImage = true;
                }

                //In original image co-ordinates
                Decimal mPixelX = 0.0m;
                Decimal mPixelY = 0.0m;
                if (bRecentreImage)
                    PicBoxXY_ToImagePixelXY((PictureBox)sender, mdScale, nMouseZoomAtPanelX, nMouseZoomAtPanelY,
                        ref mPixelX, ref mPixelY, mnZoomedImageTopLeftX, mnZoomedImageTopLeftY);

                MatToPictureBox_Zoomed(mCurrent, picOut, sDescription, true, bRecentreImage, false);
            }
        }

        private void picOut_MouseEnter(object sender, EventArgs e)
        {
            btnDeblur.Focus();
        }

        private void chkMoffatInPasses_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void optVoigt_CheckedChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void optMoffat_CheckedChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void optGaussianDeblur_CheckedChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void optCameraCircleDeblur_CheckedChanged(object sender, EventArgs e)
        {
            UIChangedDoAutoDeblur();
        }

        private void udMoffatPasses_ValueChanged(object sender, EventArgs e)
        {
            if (chkMoffatInPasses.Checked)
                UIChangedDoAutoDeblur();
        }

        private void mSetBackColorsForOptions()
        {
            if (optCircularBlur.Checked)
            {
                grpPSF.Visible = true;
                grpPSF.BackColor = System.Drawing.SystemColors.ControlLight;
                grpMotionBlurDetails.Visible = false;
                grpMotionBlurDetails.BackColor = System.Drawing.SystemColors.Control;
                grpLayers.Visible = false;
                grpLayers.BackColor = System.Drawing.SystemColors.Control;
                if (!grpMethod.Visible)
                {
                    grpMethod.Visible = true;
                    grpMethod.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
                }
            }
            else if (optMotionBlur.Checked)
            {
                grpMotionBlurDetails.Visible = true;
                grpMotionBlurDetails.BackColor = System.Drawing.SystemColors.ControlLight;
                grpPSF.Visible = false;
                grpPSF.BackColor = System.Drawing.SystemColors.Control;
                grpLayers.Visible = false;
                grpLayers.BackColor = System.Drawing.SystemColors.Control;
                if (!grpMethod.Visible)
                {
                    grpMethod.Visible = true;
                    grpMethod.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
                }
            }
            else if (optSharpeningLayers.Checked)
            {
                grpLayers.Visible = true;
                grpLayers.BackColor = System.Drawing.SystemColors.ControlLight;
                grpPSF.Visible = false;
                grpPSF.BackColor = System.Drawing.SystemColors.Control;
                grpMotionBlurDetails.Visible = false;
                grpMotionBlurDetails.BackColor = System.Drawing.SystemColors.Control;
                grpMethod.Visible = false;
                grpMethod.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void txtImage_TextChanged(object sender, EventArgs e)
        {
            tip.SetToolTip(txtImage, txtImage.Text);
        }

        private void optCircularBlur_CheckedChanged(object sender, EventArgs e)
        {
            mSetBackColorsForOptions();
            if (optCircularBlur.Checked)
                UIChangedDoAutoDeblur();
        }

        private void optMotionBlur_CheckedChanged(object sender, EventArgs e)
        {
            mSetBackColorsForOptions();
        }

        private void optSharpeningLayers_CheckedChanged(object sender, EventArgs e)
        {
            mSetBackColorsForOptions();

            if (!mbSettingsLoaded)
                return;

            if (!mbImageInLoaded)
                return;

            this.Cursor = Cursors.WaitCursor;

            if (!mbSettingsComboLoaded && optSharpeningLayers.Checked)
                mLoadLayerSettingsCombo(cboLayerSettings);

            if (!optSharpeningLayers.Checked)
            {
                for (int i = 0; i < mLAYERS.Length; i++)
                {
                    mLAYERS[i].Dispose();
                }
                mbLayersCalc = false;
                GC.Collect();
                if (!chkAutoDeblur.Checked)
                    cboCurrent_SelectedIndexChanged(null, null);
            }
            else
            {
                mSetUpSharpeningLayers();

                if (mbLayersCalc && optSharpeningLayers.Checked && mbImageInLoaded)
                    mLayersChanged();
            }

            this.Cursor = Cursors.Default;
        }

        private void chkInvPSF_CheckedChanged(object sender, EventArgs e)
        {
            Mat mCurrentPSF = new Mat();
            float fPSFHistoryFWHM = 0.0f;

            if (optMotionBlur.Checked)
            {
                picPSFProfile.Image = null;
                return;
            }

            if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
            {
                if (chkInvPSF.Checked)
                    mCurrentPSF = mPsfInvREPAIRED;
                else
                    mCurrentPSF = mPsfREPAIRED;
                fPSFHistoryFWHM = mFWHMREPAIRED;
            }
            else
            {
                if (chkInvPSF.Checked)
                    mCurrentPSF = mPSFInvHistory[cboCurrent.SelectedIndex];
                else
                    mCurrentPSF = mPSFHistory[cboCurrent.SelectedIndex];
                fPSFHistoryFWHM = mPSFHistoryFWHM[cboCurrent.SelectedIndex];
            }

            if (chkInvPSF.Checked) //FWHM not calculated for inverse PSF
                fPSFHistoryFWHM = 0.0f;

            //Plot profile
            PlotPSFProfile(mCurrentPSF, picPSFProfile, (int)udPSFPlotWidth.Value, fPSFHistoryFWHM);
            //Display Cropped 2D PSF in PictureBox
            Mat psfCropped = PSFCroppedForDisplay(mCurrentPSF, (int)(udPSFPlotWidth.Value), 65000.0f);
            MatToPictureBox(psfCropped, picPSF, false, new System.Drawing.Point(0, 0));
            psfCropped.Dispose();
        }

        private void btnClearPSFPlot_Click(object sender, EventArgs e)
        {
            System.Drawing.Graphics g = picPSFProfile.CreateGraphics();
            g.Clear(picPSFProfile.BackColor);
        }

        Decimal mdScale = 1.0m;
        Decimal mdLastScale = 1.0m;
        private void udZoom_ValueChanged(object sender, EventArgs e)
        {
            if (mbIgnoreZoomChange)
                return;
            mdLastScale = mdScale;

            Mat mCurrent = null;
            string sDescription = "";
            if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
            {
                mCurrent = mREPAIRED;
                sDescription = msRepairedDesription;
            }
            else
            {
                mCurrent = mHistory[cboCurrent.SelectedIndex];
                sDescription = cboCurrent.Items[cboCurrent.SelectedIndex].ToString();
            }

            mdScale = udZoom.Value;
            MatToPictureBox_Zoomed(mCurrent, picOut, sDescription, true, false, false);
            btnDeblur.Focus();
        }

        private void PicBoxXY_ToImagePixelXY(PictureBox pic, Decimal fScale, 
            Decimal fPicX, Decimal fPicY, ref Decimal fImageX, ref Decimal fImageY, 
            int nImageTopLeftX, int nImageTopLeftY)
        {
            fImageX = (fPicX - pic.Location.X) / fScale + nImageTopLeftX;
            fImageY = (fPicY - pic.Location.Y) / fScale + nImageTopLeftY;
        }

        private void ImagePixelXY_ToPicBoxXY(Decimal fScale,
            ref Decimal fPicX, ref Decimal fPicY, Decimal fImageX, Decimal fImageY,
            int nImageTopLeftX, int nImageTopLeftY)
        {
            fPicX = (fImageX - nImageTopLeftX) * fScale;
            fPicY = (fImageY - nImageTopLeftY) * fScale;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int nChange = e.Delta / 100;
            Decimal dChangePerInc = 1.18m;
            Decimal dChangeToMake = 1.0m;
            if (nChange < 0)
            {
                nChange = -nChange;
                dChangePerInc = 1.0m / dChangePerInc;
            }
            for (int i = 0; i < nChange; i++)
            {
                dChangeToMake = dChangeToMake * dChangePerInc;
            }
            //Change scale
            mdLastScale = mdScale;
            mdScale = mdScale * dChangeToMake;
            Decimal nMouseZoomAtPanelX = e.X - pnlOut.Location.X;
            Decimal nMouseZoomAtPanelY = e.Y - pnlOut.Location.Y;

            //In original image co-ordinates
            Decimal mPixelX = 0.0m;
            Decimal mPixelY = 0.0m;
            PicBoxXY_ToImagePixelXY(picOut, mdLastScale, nMouseZoomAtPanelX, nMouseZoomAtPanelY,
                ref mPixelX, ref mPixelY, mnZoomedImageTopLeftX, mnZoomedImageTopLeftY);

            mbIgnoreZoomChange = true;
            if (mdScale > udZoom.Maximum)
                mdScale = udZoom.Maximum;
            if (mdScale < udZoom.Minimum)
                mdScale = udZoom.Minimum;
            udZoom.Value = mdScale;
            mbIgnoreZoomChange = false;

            if (optSharpeningLayers.Checked)
                MatToPictureBox_Zoomed(mREPAIRED, picOut, msRepairedDesription, true, true, true);
            else
            {
                Mat mCurrent = null;
                string sDescription = "";
                if (cboCurrent.SelectedItem.ToString() == "REPAIRED")
                {
                    mCurrent = mREPAIRED;
                    sDescription = msRepairedDesription;
                }
                else
                {
                    mCurrent = mHistory[cboCurrent.SelectedIndex];
                    sDescription = cboCurrent.Items[cboCurrent.SelectedIndex].ToString();
                }
                MatToPictureBox_Zoomed(mCurrent, picOut, sDescription, true, true, true);
            }

            Decimal fPicX = 0.0m;
            Decimal fPicY = 0.0m;
            ImagePixelXY_ToPicBoxXY(mdScale, ref fPicX, ref fPicY, mPixelX, mPixelY,
                mnZoomedImageTopLeftX, mnZoomedImageTopLeftY);
            picOut.Location = new System.Drawing.Point((int)Math.Round(nMouseZoomAtPanelX - fPicX, 0),
                (int)Math.Round(nMouseZoomAtPanelY - fPicY, 0));
        }

        private void pnlOut_MouseUp(object sender, MouseEventArgs e)
        {
            picOut.Focus();
        }

        private void btnOriginal_Click(object sender, EventArgs e)
        {
            cboHistory.SelectedIndex = 0;
        }

        private void btnStoreOutput_Click(object sender, EventArgs e)
        {
            if (cboCurrent.Items.Count == 0)
                return;
            this.Cursor = Cursors.WaitCursor;
            btnStoreOutput.Enabled = false;

            if (cboCurrent.Items[cboCurrent.Items.Count - 1].ToString() == "REPAIRED")
                cboCurrent.Items.RemoveAt(cboCurrent.Items.Count - 1);

            if (optSharpeningLayers.Checked)
            {
                if (mREPAIRED == null)
                    return;
                mHistory[mHistoryCurrent + 1] = mREPAIRED.Clone();
                mPSFHistory[mHistoryCurrent + 1] = mPsfREPAIRED.Clone();
                mPSFInvHistory[mHistoryCurrent + 1] = mPsfInvREPAIRED.Clone();
                mFTDisplayHistory[mHistoryCurrent + 1] = mFTDisplayREPAIRED.Clone();
                mHistoryCurrent++;
            }
            else
            {
                if (mREPAIRED == null)
                    return;

                //msRepairedDesription is already set in btnDeblur_Click
                mHistory[mHistoryCurrent + 1] = mREPAIRED.Clone();
                mPSFHistory[mHistoryCurrent + 1] = mPsfREPAIRED.Clone();
                mPSFInvHistory[mHistoryCurrent + 1] = mPsfInvREPAIRED.Clone();
                mFTDisplayHistory[mHistoryCurrent + 1] = mFTDisplayREPAIRED.Clone();
                mPSFHistoryFWHM[mHistoryCurrent + 1] = mFWHMREPAIRED;
                mHistoryCurrent++;
            }

            cboHistory.Items.Add(msRepairedDesription);
            cboCurrent.Items.Add(msRepairedDesription);

            cboHistory.SelectedIndex = mHistoryCurrent - 1;
            mbIgnoreCurrentDisplayCboChange = true;
            cboCurrent.SelectedIndex = cboCurrent.Items.Count - 1;
            mbIgnoreCurrentDisplayCboChange = false;

            lblLastStored.Visible = true;
            optPrevious.Visible = true;

            btnStoreOutput.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private void chkAutostretch_CheckedChanged(object sender, EventArgs e)
        {
            if (!mbImageInLoaded)
                return;

            if (cboCurrent.SelectedItem.ToString() != "REPAIRED")
                return;
            this.Cursor = Cursors.WaitCursor;
            if (chkAutostretch.Checked)
                mREPAIRED = StretchImagePercentiles(mREPAIRED, mnPercentilesStretchMin, mnPercentilesStretchMax);
            else
                mREPAIRED = mREPAIRED_Unstretched;

            MatToPictureBox_Zoomed(mREPAIRED, picOut, msRepairedDesription, true, false, false);
            this.Cursor = Cursors.Default;
        }

        private void mLayersChanged()
        {
            grpLayers.Enabled = false;
            CombineLayers((float)udLayersScale.Value);
            grpLayers.Enabled = true;
        }

        private void chkLayers_CheckedChanged(object sender, EventArgs e)
        {
            if (mbIgnoreLayersChanged)
                return;

            string sSender = ((CheckBox)sender).Name;
            if (sSender.EndsWith("0"))
                trkLayers0.Visible = ((CheckBox)sender).Checked;
            if (sSender.EndsWith("1"))
                trkLayers1.Visible = ((CheckBox)sender).Checked;
            if (sSender.EndsWith("2"))
                trkLayers2.Visible = ((CheckBox)sender).Checked;
            if (sSender.EndsWith("3"))
                trkLayers3.Visible = ((CheckBox)sender).Checked;
            if (sSender.EndsWith("4"))
                trkLayers4.Visible = ((CheckBox)sender).Checked;
            if (sSender.EndsWith("5"))
                trkLayers5.Visible = ((CheckBox)sender).Checked;

            int nLayersChecked = 0;
            Decimal mTrkVal = 0.0m;
            if (trkLayers0.Visible)
            {
                mTrkVal = trkLayers0.Value;
                nLayersChecked++;
            }
            if (trkLayers1.Visible)
            {
                mTrkVal = trkLayers1.Value;
                nLayersChecked++;
            }
            if (trkLayers2.Visible)
            {
                mTrkVal = trkLayers2.Value;
                nLayersChecked++;
            }
            if (trkLayers3.Visible)
            {
                mTrkVal = trkLayers3.Value;
                nLayersChecked++;
            }
            if (trkLayers4.Visible)
            {
                mTrkVal = trkLayers4.Value;
                nLayersChecked++;
            }
            if (trkLayers5.Visible)
            {
                mTrkVal = trkLayers5.Value;
                nLayersChecked++;
            }

            if (nLayersChecked == 1)
            {
                mLastTrkLayersValue = mTrkVal;
                mCurrentTrkLayersValue = mTrkVal;
            }

            if (mbImageInLoaded)
            {
                this.Cursor = Cursors.WaitCursor;
                mLayersChanged();
                this.Cursor = Cursors.Default;
            }
        }

        private Decimal mLastTrkLayersValue = 3.0m;
        private Decimal mCurrentTrkLayersValue = 3.0m;
        private void trkLayers_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void trkLayers_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void trkLayers_KeyUp(object sender, KeyEventArgs e)
        {
            if (mbIgnoreLayersChanged)
                return;
            this.Cursor = Cursors.WaitCursor;
            mCurrentTrkLayersValue = ((TrackBar)sender).Value;
            trkLayersChange();
            mLayersChanged();
            this.Cursor = Cursors.Default;
        }

        private void trkLayers_MouseUp(object sender, MouseEventArgs e)
        {
            if (mbIgnoreLayersChanged)
                return;
            this.Cursor = Cursors.WaitCursor;
            mCurrentTrkLayersValue = ((TrackBar)sender).Value;
            trkLayersChange();
            mLayersChanged();
            this.Cursor = Cursors.Default;
        }

        private void trkLayers_Scroll(object sender, EventArgs e)
        {
            tip.SetToolTip((TrackBar)sender, ((TrackBar)sender).Value.ToString());
        }

        private void trkLayersChange()
        {
            //If only one layer in use then change scale
            int nLayersUsed = 0;
            if (trkLayers0.Visible)
                nLayersUsed++;
            if (trkLayers1.Visible)
                nLayersUsed++;
            if (trkLayers2.Visible)
                nLayersUsed++;
            if (trkLayers3.Visible)
                nLayersUsed++;
            if (trkLayers4.Visible)
                nLayersUsed++;
            if (trkLayers5.Visible)
                nLayersUsed++;

            if (nLayersUsed == 1)
            {
                Decimal mDiff = 1.0m + (mCurrentTrkLayersValue - mLastTrkLayersValue) / trkLayers0.Maximum;
                mbIgnoreLayersChanged = true;
                Decimal mNewValue = Math.Round(mLastLayersScale * mDiff * mDiff, 2);
                if (mNewValue > udLayersImageScale.Maximum)
                    mNewValue = udLayersImageScale.Maximum;
                if (mNewValue < udLayersImageScale.Minimum)
                    mNewValue = udLayersImageScale.Minimum;
                udLayersScale.Value = mNewValue;
                mbIgnoreLayersChanged = false;
            }
        }

        private void btnResetLayers_Click(object sender, EventArgs e)
        {
            mbIgnoreLayersChanged = true;
            trkLayers0.Value = 1;
            chkLayers0.Checked = false;
            trkLayers1.Value = 1;
            chkLayers1.Checked = false;
            trkLayers2.Value = 1;
            chkLayers2.Checked = false;
            trkLayers3.Value = 1;
            chkLayers3.Checked = false;
            trkLayers4.Value = 1;
            chkLayers4.Checked = false;
            trkLayers5.Value = 1;
            chkLayers5.Checked = false;
            udLayersScale.Value = 2.0m;
            udLayersNoiseControl.Value = 0.6m;
            udLayersImageScale.Value = 1.0m;
            trkLayers0.Visible = false;
            trkLayers1.Visible = false;
            trkLayers2.Visible = false;
            trkLayers3.Visible = false;
            trkLayers4.Visible = false;
            trkLayers5.Visible = false;
            mCurrentTrkLayersValue = 1.0m;
            mLastTrkLayersValue = 1.0m;
            mLastLayersScale = 2.0m;

            mbIgnoreLayersChanged = false;
            this.Cursor = Cursors.WaitCursor;
            mLayersChanged();
            this.Cursor = Cursors.Default;
        }

        private Decimal mLastLayersScale = 3.0m;
        private void udLayersScale_ValueChanged(object sender, EventArgs e)
        {
            if (!mbImageInLoaded)
                return;
            if (mbIgnoreLayersChanged)
                return;

            Decimal mCurVal = udLayersScale.Value;
            if (mCurVal < 1.0m)
                udLayersScale.Increment = 0.1m;
            else if (mCurVal < 2.0m)
                udLayersScale.Increment = 0.2m;
            else if (mCurVal < 5.0m)
                udLayersScale.Increment = 0.5m;
            else if (mCurVal < 10.0m)
                udLayersScale.Increment = 1.0m;
            else if (mCurVal < 20.0m)
                udLayersScale.Increment = 2.0m;
            else
                udLayersScale.Increment = 5.0m;

            mLastLayersScale = udLayersScale.Value;
            mLastTrkLayersValue = mCurrentTrkLayersValue;
            this.Cursor = Cursors.WaitCursor;
            mLayersChanged();
            this.Cursor = Cursors.Default;
        }

        private void udLayersImageScale_ValueChanged(object sender, EventArgs e)
        {
            if (!mbImageInLoaded)
                return;
            if (mbIgnoreLayersChanged)
                return;
            //Force layers to be recalculated
            mbLayersCalc = false;
            optSharpeningLayers_CheckedChanged(null, null);
        }

        private void udLayersNoiseControl_ValueChanged(object sender, EventArgs e)
        {
            if (!mbImageInLoaded)
                return;
            if (mbIgnoreLayersChanged)
                return;
            //Force layers to be recalculated
            mbLayersCalc = false;
            optSharpeningLayers_CheckedChanged(null, null);
        }

        private void trkIterations_MouseUp(object sender, MouseEventArgs e)
        {
            mIterationsValueChanged();
        }

        private void trkIterations_KeyUp(object sender, KeyEventArgs e)
        {
            mIterationsValueChanged();
        }

        private void chkFiledRotationDeblur_CheckedChanged(object sender, EventArgs e)
        {
            udMotionBlurLength.Enabled = !chkFiledRotationDeblur.Checked;
            udMotionBlurAngle.Enabled = !chkFiledRotationDeblur.Checked;
            chkRotateImage.Enabled = !chkFiledRotationDeblur.Checked;
        }

        private void cboKernelSharpeningLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mbIgnoreKernelSharpeningLayersChanged)
                return;
            if (optSharpeningLayers.Checked)
            {
                mbLayersCalc = false;
                mSetUpSharpeningLayers();
                mLayersChanged();
            }
        }

        private void btnFourierTransform_Click(object sender, EventArgs e)
        {
            if (!mbImageInLoaded)
                return;

            this.Cursor = Cursors.WaitCursor;

            Mat img= new Mat();

            if (chkPSF.Checked)
            {
                if (mHistoryCurrent == 0)
                    img = mPsfREPAIRED;
                else
                    img = mPSFHistory[mHistoryCurrent];
            }
            else
                img = mHistory[mHistoryCurrent];

            if (img == null)
            {
                this.Cursor = Cursors.Default;
                return;
            }

            int nChannels = img.Channels();

            Mat[] imgCh = new Mat[0];
            if (nChannels > 1)
            {
                imgCh = new Mat[nChannels];
                Cv2.Split(img, out imgCh);
                img = imgCh[1]; //Use green from RGB image
            }

            Mat FTReal = new Mat(); //Disposed
            Mat FTImag = new Mat(); //Disposed
            DFT(img, null, ref FTReal, ref FTImag, DftFlags.None);

            if (mbDebug)
            {
                StringBuilder sbDebug = new StringBuilder();
                Mat Mag = FTReal.EmptyClone();
                Cv2.Magnitude(FTReal, FTImag, Mag);
                for (int x = 0; x <= mnWidth/2; x++)
                {
                    sbDebug.Append(x.ToString() + "\t" + FTReal.At<float>(0,x).ToString() 
                        + "\t" + FTImag.At<float>(0, x).ToString() + "\t" +
                        Mag.At<float>(0, x).ToString() + "\r\n");
                }
                txtDebug.Text = sbDebug.ToString();
                Mag.Dispose();
            }

            mREPAIRED = mLogFFT(FTReal, FTImag);

            if (nChannels > 1)
            {
                for (int nCh = 0; nCh < nChannels; nCh++)
                {
                    imgCh[nCh] = mREPAIRED;
                }
                Cv2.Merge(imgCh, mREPAIRED);
            }

            msRepairedDesription = "Fourier Transform";


            MatToPictureBox_Zoomed(mREPAIRED, picOut, msRepairedDesription, true, false, false);
            if (cboCurrent.Items[cboCurrent.Items.Count - 1].ToString() != "REPAIRED")
                cboCurrent.Items.Add("REPAIRED");

            FTReal.Dispose();
            FTImag.Dispose();

            if (cboCurrent.SelectedIndex != cboCurrent.Items.Count - 1)
            {
                mbIgnoreCurrentDisplayCboChange = true;
                cboCurrent.SelectedIndex = cboCurrent.Items.Count - 1;
                mbIgnoreCurrentDisplayCboChange = false;
            }

            this.Cursor = Cursors.Default;
        }

        private FormWindowState LastWindowState = FormWindowState.Normal;
        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == LastWindowState)
                return;

            LastWindowState = WindowState;
            FormSizeChange();
        }

        private void FormSizeChange()
        {
            int nExtraX = this.Size.Width - this.MinimumSize.Width;
            int nExtraY = this.Size.Height - this.MinimumSize.Height;

            pnlOut.Size = new System.Drawing.Size(510 + nExtraX, 510 + nExtraY);
            picOut.Size = new System.Drawing.Size(510 + nExtraX, 510 + nExtraY);

            mnMaxImageWidth = picOut.Width * 2;
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            //Manual changes to size only
            FormSizeChange();
        }

        private void cboZoomMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboZoomMode.SelectedIndex == 0)
                interpolationFlags = InterpolationFlags.Linear;
            else if (cboZoomMode.SelectedIndex == 1)
                interpolationFlags = InterpolationFlags.Cubic;
            else if (cboZoomMode.SelectedIndex == 2)
                interpolationFlags = InterpolationFlags.Nearest;
            else if (cboZoomMode.SelectedIndex == 3)
                interpolationFlags = InterpolationFlags.Lanczos4;

            if (mbImageInLoaded)
            {
                //Refresh image
                cboCurrent_SelectedIndexChanged(null, null);
            }
        }

        //NOTES
        //https://docs.opencv.org/3.4/d1/dfd/tutorial_motion_deblur_filter.html
        //https://docs.opencv.org/3.4/de/d3c/tutorial_out_of_focus_deblur_filter.html
        //http://yuzhikov.com/projects.html
        //http://www.robots.ox.ac.uk/~az/lectures/ia/lect3.pdf
        //F:/sd/~current/Research/Astronomy/Telescope/Processing/Deconvolution/MotionBlur_Wiener.pdf

        //A normalization of 1/SQRT(No of elements) for both the DFT and IDFT, for instance, makes the transforms unitary
        //But 1/N for DFT and 1 for 1DFT would also be unitary

        //Temp extra functions
        //Extra for animated gif load
        System.Drawing.Image mgifImg = null;
        System.Drawing.Imaging.FrameDimension mdimension = null;
        System.Drawing.Image[] mFrames = null;
        private void btnFFTW_Click(object sender, EventArgs e)
        {
            //See CNotUsed.cs
        }

        private void trkGif_Scroll(object sender, EventArgs e)
        {
            // Return an Image at a certain index
            mgifImg.SelectActiveFrame(mdimension, trkGif.Value);
        }

        private void btnLimb_Click(object sender, EventArgs e)
        {
            //See CNotUsed.cs
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            double dRotateTotal = 1.5d; //Deg anticlockwise
            int nSteps = 200;
            double dRotate = 0.0d;
            Mat imgF = mHistory[0].Clone();
           
            Mat imgOut = imgF.EmptyClone();
            Point2f fCentre = new Point2f(mnWidth / 2.0f, mnHeight / 2.0f);
            Mat rot_mat = Cv2.GetRotationMatrix2D(fCentre, dRotate, 1.0d);
            Mat rotated = imgF.EmptyClone();
            for (int nRotate = 0; nRotate < nSteps; nRotate++)
            {
                dRotate = dRotateTotal * nRotate / nSteps;
                rot_mat = Cv2.GetRotationMatrix2D(fCentre, dRotate, 1.0d);
                Cv2.WarpAffine(imgF, rotated, rot_mat, imgF.Size()); //Crops rotated
                imgOut = imgOut + rotated / nSteps;
            }

            SaveImage(imgOut, @"F:\temp\mask\rotated_IC434.tif");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
           //See CNotUsed.cs
        }
    }

    public class PixelBox : PictureBox
    {
        //To get pixelated PSF PictureBox, otherwise it uses interpolation when stretching a small image
        public PixelBox()
        {
            InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        [Category("Behavior")]
        [DefaultValue(InterpolationMode.Default)]
        public InterpolationMode InterpolationMode { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(pe);
        }
    }
}
