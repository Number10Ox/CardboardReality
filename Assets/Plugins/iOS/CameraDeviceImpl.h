#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

@interface CameraDevice : NSObject
{
	AVCaptureDevice *captureDevice;
}

- (void)configure;
- (float)getMinExposureTargetBias;
- (float)getMaxExposureTargetBias;
- (float)getExposureTargetBias;
- (void)setExposureTargetBias:(float)bias;
@end

