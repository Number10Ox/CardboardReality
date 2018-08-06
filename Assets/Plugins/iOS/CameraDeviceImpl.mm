
#import "CameraDeviceImpl.h"

@implementation CameraDevice

- (id)init
{
    self = [super init];

    NSArray *captureDeviceType = @[AVCaptureDeviceTypeBuiltInWideAngleCamera];
	AVCaptureDeviceDiscoverySession *captureDevice = [AVCaptureDeviceDiscoverySession discoverySessionWithDeviceTypes:captureDeviceType mediaType:AVMediaTypeVideo position:AVCaptureDevicePositionUnspecified];
    self->captureDevice = captureDevice.devices.firstObject;

    return self;
}

- (void)configure
{
	[self->captureDevice lockForConfiguration:nil];
	[self->captureDevice setFocusMode:AVCaptureFocusModeAutoFocus];
	[self->captureDevice setExposureMode:AVCaptureExposureModeContinuousAutoExposure];
	[self->captureDevice unlockForConfiguration];
}

- (void)dealloc
{
}

- (float)getMinExposureTargetBias
{
    return self->captureDevice.minExposureTargetBias;
}

- (float)getMaxExposureTargetBias
{
    return self->captureDevice.maxExposureTargetBias;
}

- (float)getExposureTargetBias
{
    return self->captureDevice.exposureTargetBias;
}

- (void)setExposureTargetBias:(float)bias
{
    void (^completionHandler)(CMTime syncTime) = ^(CMTime syncTime) {
        [self->captureDevice unlockForConfiguration];
    };
    
    [self->captureDevice lockForConfiguration:nil];
    [self->captureDevice setExposureTargetBias:bias completionHandler:completionHandler];
}

@end

static CameraDevice *sCameraDeviceObject = nil;

extern "C" {

	void _CameraDeviceInitialize()
	{
		if (sCameraDeviceObject == nil)
		{
			sCameraDeviceObject = [[CameraDevice alloc] init];
			[sCameraDeviceObject configure];
		}
	}
    
	float _CameraDeviceGetMinExposureTargetBias()
	{
		return [sCameraDeviceObject getMinExposureTargetBias];
	}

    float _CameraDeviceGetMaxExposureTargetBias()
    {
        return [sCameraDeviceObject getMaxExposureTargetBias];
    }

    float _CameraDeviceGetExposureTargetBias()
    {
        return [sCameraDeviceObject getExposureTargetBias];
    }
    
    void _CameraDeviceSetExposureTargetBias(float bias)
	{
        [sCameraDeviceObject setExposureTargetBias:bias];
	}
}



