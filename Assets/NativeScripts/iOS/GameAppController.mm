
#import "UnityAppController.h"
#import "GameAppController.h"
#import "AppDelegateListener.h"

IMPL_APP_CONTROLLER_SUBCLASS (GameAppController)
@implementation GameAppController

- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
    [super application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
    ::printf("GameAppController didRegisterForRemoteNotificationsWithDeviceToken \n");
}
- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
    ::printf("GameAppController applicationDidFinishLaunching()\n");
    return YES;
}
@end

