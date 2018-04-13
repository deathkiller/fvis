#include "stdafx.h"

#if defined(_WIN32)
BOOL APIENTRY DllMain(HMODULE hInstance, DWORD dwReason, LPVOID lpReserved) {
    if (dwReason == DLL_PROCESS_ATTACH) {
        //_hInst = hInstance;
        DisableThreadLibraryCalls(hInstance);
    }
    return TRUE;
}
#endif