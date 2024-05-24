#ifndef IKLASA2_H
#define IKLASA2_H

#include<windows.h>
DEFINE_GUID(IID_IKlasa2, 0xd4a18c38, 0x507d, 0x458f, 0xb4, 0x7f, 0xd, 0xf8, 0x9b, 0xde, 0xc7, 0xe4);
DEFINE_GUID(CLSID_Klasa2, 0x67417a67, 0x96f, 0x45ee, 0x80, 0x63, 0x4d, 0x20, 0x6f, 0x59, 0x85, 0xb5);

class IKlasa : public IUnknown {
public:
	virtual HRESULT STDMETHODCALLTYPE Test(const char *napis) = 0;
};

#endif
