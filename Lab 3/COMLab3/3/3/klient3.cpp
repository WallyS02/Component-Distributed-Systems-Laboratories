#include<windows.h>
#include<stdio.h>
#import "klasa.tlb" no_namespace

int main() {

	CoInitializeEx(NULL, COINIT_MULTITHREADED);
	IKlasa *s;
	HRESULT rv;
	rv = CoCreateInstance(__uuidof(Klasa), NULL, CLSCTX_ALL, __uuidof(IKlasa), (void **)&s);
	if (SUCCEEDED(rv)) {
		s->Test("Testowanie, zadanie 3 ok!");
		s->Release();
	}
	else {
		printf("nie pobrano instancji klasy");
	}


	CoUninitialize();

	return 0;
};
