// COM Checker.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "COM Checker.h"

//int APIENTRY _tWinMain(HINSTANCE hInstance,
//                       HINSTANCE hPrevInstance,
//                       LPTSTR    lpCmdLine,
//                       int       nCmdShow)
//{
//	UNREFERENCED_PARAMETER(hInstance);
//	UNREFERENCED_PARAMETER(hPrevInstance);
//	UNREFERENCED_PARAMETER(nCmdShow);
int main()
{
 	// Initialise COM first, otherwise there's no point.
	HRESULT hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
	if (hr != S_OK) {
		std::tcout << "CoInitializeEx failed: " << hr << std::endl;
		return hr;
	}

	// Split the command line into arguments. Let Windows' Shell do the leg-work.
	int nArgs;
	LPWSTR* szArgv = CommandLineToArgvW(GetCommandLine(), &nArgs);
	if (NULL == szArgv) {
		std::tcout << "CommandLineToArgvW failed: " << GetLastError() << std::endl;
		return 1;
	}

	// Parse the command line:
	//   /classid {...}
	//   /interfaces {...} {...} ...
	IID classid;
	std::vector<IID> intfList;
	for(int i = 0; i < nArgs; i++) {
		std::tstring arg(szArgv[i]);
		if (arg.compare(_T("/classid")) == 0) {
			if (++i >= nArgs) {
				std::tcout << "/classid argument missing value." << std::endl;
				return 2;
			}
			hr = IIDFromString(szArgv[i], &classid);
			if (hr != S_OK) {
				std::tcout << "IIDFromString for " << szArgv[i] << " failed: " << hr << std::endl;
				return hr;
			}
		} else if (arg.compare(_T("/interface")) == 0) {
			if (++i >= nArgs) {
				std::tcout << "/interface argument missing value." << std::endl;
				return 2;
			}
			IID intf;
			hr = IIDFromString(szArgv[i], &intf);
			if (hr != S_OK) {
				std::tcout << "IIDFromString for " << szArgv[i] << " failed: " << hr << std::endl;
				return hr;
			}
			intfList.push_back(intf);
		} else if (arg.compare(_T("/interfaces")) == 0) {
			while (++i < nArgs) {
				IID intf;
				hr = IIDFromString(szArgv[i], &intf);
				if (hr != S_OK) {
					std::tcout << "IIDFromString for " << szArgv[i] << " failed: " << hr << std::endl;
					return hr;
				}
				intfList.push_back(intf);
			}
		}
	}

	// We're done with the argument list.
	LocalFree(szArgv);

	// If we have no interfaces to check, just use IUnknown.
	if (intfList.size() == 0) {
		intfList.push_back(IID_IUnknown);
	}

	// Construct the object as IUnknown.
	IUnknown* object;
	hr = CoCreateInstance(classid, NULL, CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER | CLSCTX_LOCAL_SERVER, IID_IUnknown, (void**)&object);

	{
		// Output a line of text of this format, with interface class ID and HRESULT:
		//   {CLSID} 0x00000000
		LPOLESTR classString;
		StringFromIID(classid, &classString);
		std::tstring output(50, 0);
		_stprintf_s(&output[0], output.length(), _T("%s 0x%.8x"), classString, hr);
		std::tcout << output.c_str() << std::endl;
	}

	if (hr != S_OK) {
		std::tcout << "CoCreateInstance failed: " << hr << std::endl;
		return 0;
	}

	for (std::vector<IID>::size_type i = 0; i < intfList.size(); i++) {
		// Try and QI to the given interface. If it succeeds, we must Release() as QueryInterface AddRef()s.
		void* objectIntf;
		hr = object->QueryInterface(intfList[i], &objectIntf);
		if (hr == S_OK) {
			object->Release();
		}
		objectIntf = NULL;

		// Output a line of text of this format, with interface class ID and HRESULT:
		//   {IID} 0x00000000
		LPOLESTR intfString;
		StringFromIID(intfList[i], &intfString);
		std::tstring output(50, 0);
		_stprintf_s(&output[0], output.length(), _T("%s 0x%.8x"), intfString, hr);
		std::tcout << output.c_str() << std::endl;
	}

	// Release the IUnknown object.
	object->Release();
	object = NULL;

	// We're done, shut down COM.
	CoUninitialize();

	return 0;
}
