/*
 * SPDX-FileCopyrightText: 2025 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

#include <initializer_list>
#include <string>
#include <windows.h>

namespace {
std::initializer_list<std::wstring> sampleFileNames = {
  // an empty path
  L"\0",
  // a trivially short path
  L"C:\\Users\\Username\\AppData\\Roaming\\Notepad++\\session.xml",
  // an internationalized path
  L"D:\\用户\\用户名\\AppData\\漫游\\Notepad++\\会话.xml",
  // path == MAX_PATH - 1 wide characters
  L"E:"
  L"\\3e2t9uccIJ90JnhCaH56740Y0K4\\cjpPh0p3BD8YxJcyTz2mv1HssjITN38\\C6UjtMeC6IF"
  L"YH80QlH52Fz8defWrBzteO\\gR5gCqlzYYDrX36xcArpzRJalEQBZtBC\\p1b0ZYll6XkC92I6u"
  L"O66291F26\\2Aiyr5RpBJz67aOh0FzWVzD3VykQp5ILJq\\eJ1zAwlw6v8ZBDWpsge6eN7lvk7S"
  L"v5K3\\m9FqMlg7S4F6VP2g5rJykAe4r67qw.txt",
  // path > MAX_PATH wide characters
  L"Z:"
  L"\\3e2t9uccIJ90JnhCOaH40Y0K4Xy0gTAt\\cjpPh0p3BD8YxJcyTz2mv1HssjITJN38\\C6Ujt"
  L"MeC6IFYH80QlH2Fz8defWrBzteO\\gR5gCqlzYYDrX3xcArpzRJalEQBZtBCE\\p1b0ZYllXkC9"
  L"2I6uO66291F26bIHTTpe\\2Aiyr5RpBJzaOh0FzWVzD3VykQp5ILJq\\eJ1zAwlw6v8ZBDWpsge"
  L"6eN7lvk7Sv5K3\\m9FqMlg7S3F6VP2g5rJykAFWMNrN1EM7\\OCpjZ5tXo5TB23yBiQMktVKYN9"
  L"esJ58R\\VlDBzI5Tc53CMO0cXn7fb2D05XgytiUw.tmp",
};
}

extern "C" __declspec(dllexport) constexpr int
GetFileCount() noexcept
{
  return static_cast<int>(sampleFileNames.size());
}

extern "C" __declspec(dllexport) void
GetFileNames(_In_ int nbFileNames, _Inout_ wchar_t** fileNames)
{
  if (fileNames == nullptr)
    return;
  constexpr int maxLength = MAX_PATH - 1;
  intptr_t j = 0LL;
  for (auto it = sampleFileNames.begin();
       it != sampleFileNames.end() && j < nbFileNames;
       ++it, ++j) {
    if (fileNames[j] == nullptr)
      continue;
    lstrcpyn(fileNames[j], it->c_str(), maxLength);
    fileNames[j][std::min(static_cast<int>(it->size()), maxLength)] = L'\0';
  }
}
