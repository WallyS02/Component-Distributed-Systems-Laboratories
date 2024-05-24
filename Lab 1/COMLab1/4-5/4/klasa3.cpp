#include<windows.h>
#include <stdio.h>
#include"IKlasa3.h"


extern volatile ULONG usageCount;


Klasa3::Klasa3() {
	InterlockedIncrement(&usageCount);
	m_ref = 0;
};


Klasa3::~Klasa3() {
	InterlockedDecrement(&m_ref);
};


ULONG STDMETHODCALLTYPE Klasa3::AddRef() {
	return InterlockedIncrement(&m_ref);
};


ULONG STDMETHODCALLTYPE Klasa3::Release() {
	ULONG rv = InterlockedDecrement(&m_ref);
	if (rv == 0) delete this;
	return rv;
};


HRESULT STDMETHODCALLTYPE Klasa3::QueryInterface(REFIID iid, void** ptr) {
	if (ptr == NULL) return E_POINTER;
	if (IsBadWritePtr(ptr, sizeof(void*))) return E_POINTER;
	*ptr = NULL;
	if (iid == IID_IUnknown) *ptr = this;
	if (iid == IID_IKlasa3) *ptr = this;
	if (*ptr != NULL) { AddRef(); return S_OK; };
	return E_NOINTERFACE;
};

HRESULT STDMETHODCALLTYPE Klasa3::Test(const char* napis) {
	printf(napis);
	return S_OK;
};