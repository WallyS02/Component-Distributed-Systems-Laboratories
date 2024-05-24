#include<windows.h>

DEFINE_GUID(IID_IKlasa3, 0x6c18712a, 0x4ecf, 0x4adb, 0x9a, 0x91, 0x79, 0xf1, 0x3b, 0x17, 0xf4, 0x9);
DEFINE_GUID(CLSID_Klasa3, 0xd41b2bb6, 0x8973, 0x4dbd, 0x8a, 0x92, 0xb1, 0xa8, 0x1d, 0x3f, 0x44, 0x29);

class IKlasa3 : public IUnknown {
public:
	virtual HRESULT STDMETHODCALLTYPE Test(const char* napis) = 0;
};

class Klasa3 : public IKlasa3 {
public:
	Klasa3();
	~Klasa3();
	virtual ULONG STDMETHODCALLTYPE AddRef();
	virtual ULONG STDMETHODCALLTYPE Release();
	virtual HRESULT STDMETHODCALLTYPE QueryInterface(REFIID iid, void** ptr);
	virtual HRESULT STDMETHODCALLTYPE Test(const char* napis);

private:
	volatile ULONG m_ref;
};