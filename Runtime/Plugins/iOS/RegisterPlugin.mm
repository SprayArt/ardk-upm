// Put this into LightshipArPlugin/Runtime/Plugins/iOS

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "IUnityInterface.h"

// UI loaded observer from https://blog.eppz.eu/override-app-delegate-unity-ios-macos-2/
@interface RegisterPlugin : NSObject
@end

__strong RegisterPlugin *_instance;

@implementation RegisterPlugin
+(void)load
{
    NSLog(@"[NianticLightship load]");
    _instance = [RegisterPlugin new];
    [[NSNotificationCenter defaultCenter] addObserver:_instance
                                             selector:@selector(unityWillStart:)
                                                 name:@"UnityWillStart"
                                               object:nil];
}
-(void)unityWillStart:(NSNotification*) notification
{
    NSLog(@"[NianticLightship unityWillStart:%@]", notification);
    UnityRegisterRenderingPluginV5(&UnityPluginLoad, &UnityPluginUnload);
}
@end
