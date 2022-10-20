//
//  NativeHelper.m
//  MyJSGame-mobile
//
//  Created by shihongyang on 2021/10/12.
//

#import "NativeHelper.h"
#import <StoreKit/StoreKit.h>

@implementation NativeHelper

const char* GetClipboardContent()
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    if(pasteboard.string == NULL)
    {
        const char* r = "";
        return r;
    }
    const char *str = [pasteboard.string UTF8String];
    char* res = (char*)malloc(strlen(str) + 1);
    strcpy(res, str);
    return res;
}

//复制到剪贴板
void CopyToClipboard(const char * text)
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = [[NSString alloc] initWithUTF8String:text];;
}

void VibrateFeedback(int v)
{
    if (@available(iOS 10.0, *)) {
        
        UIImpactFeedbackStyle style = UIImpactFeedbackStyleHeavy;
        if(v == 0)
        {
            style = UIImpactFeedbackStyleLight;
        }
        else if(v == 1)
        {
            style = UIImpactFeedbackStyleMedium;
        }
        
        UIImpactFeedbackGenerator *feedBackGenertor = [[UIImpactFeedbackGenerator alloc] initWithStyle:style];
        [feedBackGenertor impactOccurred];
    }
}

void RequestReview(const char * appID)
{
   if([SKStoreReviewController respondsToSelector:@selector(requestReview)]) {// iOS 10.3 以上支持
       [SKStoreReviewController requestReview];
   } else { // iOS 10.3 之前的使用这个
       NSString *appId = [[NSString alloc] initWithUTF8String:appID];
       NSString  * nsStringToOpen = [NSString  stringWithFormat: @"itms-apps://itunes.apple.com/app/id%@?action=write-review",appId];//替换为对应的APPID
       [[UIApplication sharedApplication] openURL:[NSURL URLWithString:nsStringToOpen]];
   }
}

@end
