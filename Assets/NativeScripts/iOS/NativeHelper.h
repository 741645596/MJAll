//
//  Header.h
//  MyJSGame-mobile
//
//  Created by John on 2018/5/23.
//

#ifndef NativeHelper_h
#define NativeHelper_h

@interface NativeHelper : NSObject

extern "C" {
    const char* GetClipboardContent();
    void CopyToClipboard(const char * text);
    void VibrateFeedback(int v);
    void RequestReview(const char * appID);
}

@end

#endif /* Header_h */
