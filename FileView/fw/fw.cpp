// fw.cpp : Этот файл содержит функцию "main". Здесь начинается и заканчивается выполнение программы.
//
#include <iostream>
#include <fstream>
#include <sstream>
#include <iomanip>
#include <numeric>
#include <algorithm>
#include <regex>
#include <vector>
#include <filesystem>

using namespace std;
using namespace filesystem;
/*
-показывать содержимое дисков,
 -создавать папки/файлы,
 -удалять папки/файлы,
 -переименовывать папки/файлы,
 -копировать/переносить папки/файлы,
 -размер папки/файла,
 -производить поиск по маске (с поиском по подпапкам) и так далее.
*/
//-----------------------
struct say 
{
    void operator()(const char* str) 
    {
        cout << str;
    }
	void operator()(const wstring& obj) 
	{
		wcout << obj << '\n';
	}
	void operator()(const path& obj)
	{
		wcout << obj.wstring() << '\n';
	}
};
say $;
string buf;
path point;
static list<wchar_t> logDrive;
const directory_options neatly_dir = (directory_options::none | directory_options::follow_directory_symlink | directory_options::skip_permission_denied);
error_code err;
//---start block seatings-------------------------------------------------------------------------------------------------------
static void setDiskList(list<wchar_t>& list)
{
    path point;
    wchar_t l = L'A';
    wstring disk; disk.push_back(l); disk.push_back(L':');
    error_code ec;
    do
    {
        point = disk;
        if (exists(status(point, ec)))
            list.push_back(l);
        l++;
        disk.pop_back();
        disk.pop_back();
        disk.push_back(l);
        disk.push_back(L':');
    } while (l != L'[');
}
static void setDiskRoot(const string& str, const list<wchar_t>& drives, path& point)
{
    for (auto i = drives.begin(); i != drives.end(); ++i)
    {
		if (*i == str.at(0) && str.length()==1) { point = str; point+=L":\\"; wcout << "disk has set\n"; return; }
    }
}
static wstring size_string(unsigned long long int size)
{/*вспомогательная функция принимает целочисленный размер файла и преобразует его в читабельный вид. Мы просто проигнорируем
точку при делении чисел и округлим их до ближайшего значения кило-, мег аили гигабайт.*/
    wstringstream ss;
    if (size >= 1000000000000) {
        ss << (size / 1000000000000) <<" Tb";
    }
    else if (size >= 1000000000) {
        ss << (size / 1000000000) << " Gb";
    }
    else if (size >= 1000000) {
        ss << (size / 1000000) << " Mb";
    }
    else if (size >= 1000) {
        ss << (size / 1000) << " Kb";
    }
    else { ss << size << " B"; }
    return ss.str();
}
static void showDiskInfo(list<wchar_t>& _list)
{
    space_info root;
    wstring l;
    wcout << "\n--------------------------------------\n";
    wcout << setw(6) << "Disk :" <<"  "<< setw(10) << "Capacity :" << " " << setw(10) << "Free :\n\n";
    for (auto i=_list.begin();i!=_list.end();++i )
    {
        l.push_back(*i); l.push_back(L':');
        root = space(l);
        wcout << setw(6) << l <<"  "<<setw(10) << size_string(root.capacity) << " " << setw(10) << size_string(root.free) << "\n";
        l.pop_back(); l.pop_back();
    }
    wcout << "\n--------------------------------------\n";
}
//---end block seatings----------------------------------------------------------------------------------------------------------
static tuple<path, file_status, size_t> file_info(const directory_entry& entry)
{/*принимает ссылку на объект  directory_entry  и извлекает из нее путь, а также объект  file_status
(с помощью функции  status), который содержит тип файла и информацию о правах.Наконец, она извлекает и размер записи, если это обычный файл.
Для каталогов и особых файлов мы просто возвращаем значение  0 . Вся информация упаковывается в кортеж.*/
    const auto fs(status(entry));
    return { entry.path(), fs, is_regular_file(fs) ? file_size(entry.path()) : 0u };
}
static char type_char(file_status fs)
{/*Путь может представлять не только каталоги и простые текстовые/бинарные файлы. Операционные 
 системы предоставляют множество разнообразных типов, которые абстрагируют что-то еще, 
 например интерфейсы аппаратных устройств в виде так называемых символьных/блочных файлов. 
 В библиотеке для работы с файловой системой, расположенной в STL, есть множество функций-предикатов
для них. Таким образом, можно вернуть букву  'd'  для каталогов, букву  'f'  для обычных файлов и т. д.:*/
    if (is_directory(fs)) { return 'd'; }
    else if (is_symlink(fs)) { return 'l'; }
    else if (is_character_file(fs)) { return 'c'; }
    else if (is_block_file(fs)) { return 'b'; }
    else if (is_fifo(fs)) { return 'p'; }
    else if (is_socket(fs)) { return 's'; }
    else if (is_other(fs)) { return 'o'; }
    else if (is_regular_file(fs)) { return 'f'; }
    return '?';
}
static string rwx(perms p)
{/*принимает переменную  perms  (просто возвращающую тип класса перечисления из библиотеки
для работы с файловой системой) и возвращает строку наподобие  "rwxrwxrwx" , которая описывает настройки прав для файла. Первая группа символов  "rwx" 
описывает права на чтение, запись и исполнение (read, write and execution) для владельца файла. Следующая группа описывает те же права для всех пользо-
вателей, являющихся частью пользовательской группы, к которой принадлежитфайл. Последняя группа символов описывает эти же права для всех остальных.
Строка  "rwxrwxrwx"  означает, что все пользователи могут получить доступ к объекту любым способом; строка  "rw-r--r--"  — что только владелец файла
может читать и изменять его, а все остальные — только читать. Мы просто создадим строку на основе этих значений бит за битом. Лямбда-выражение по-
может выполнить повторяющуюся работу, связанную с проверкой, содержит ли переменная  p  типа  perms  конкретный бит владельца, и возвратит символ  '-'  илисоответствующую букву*/
    auto check([p](perms bit, char c) {
        return (p & bit) == perms::none ? '-' : c;
        });
    return { check(perms::owner_read, 'r'),
    check(perms::owner_write, 'w'),
    check(perms::owner_exec, 'x'),
    check(perms::group_read, 'r'),
    check(perms::group_write, 'w'),
    check(perms::group_exec, 'x'),
    check(perms::others_read, 'r'),
    check(perms::others_write, 'w'),
    check(perms::others_exec, 'x') };
}
static vector<pair<size_t, string>> matches(const path& p, const regex& re)
{/*принимает путь к файлу и объект, содержащий регулярное выражение, описывающее искомый шаблон.
Затем создаем экземпляр вектора, который будет включать пары, состоящие из номера строк и их содержимого. 
Кроме того, создадим экземпляр объекта файлового потока ввода, из которого будем считывать содержимое и сравнивать
его с шаблоном строка за строкой*/
    vector<pair<size_t, string>> d;
    ifstream is{ p.c_str() };
    string s;
    /*Пройдем по файлу строка за строкой с помощью функции  std::getline . 
    Функция regex_search  возвращает значение  true  при условии, что строка содержит наш шаблон. 
    Если это именно так, то поместим в вектор номер строки и саму строку.
    Наконец, вернем все найденные совпадения*/
    for (size_t line{ 1 }; std::getline(is, s); ++line) {
        if (regex_search(begin(s), end(s), re)) {
            d.emplace_back(line, move(s));
        }
    }
    return d;
}
vector<wstring> getAllFilesInDir(wstring& dirPath,const vector<wstring> dirSkipList = { })
{
    // Create a vector of string
    vector<wstring> listOfFiles;
        // Check if given path exists and points to a directory
    wstring find = L"Вадим.txt";
    wstring now;
    if (exists(dirPath) && is_directory(dirPath))
    {
        const directory_options neatly = (directory_options::none | directory_options::follow_directory_symlink |
            directory_options::skip_permission_denied);

        for (auto i = recursive_directory_iterator(dirPath, neatly); i != recursive_directory_iterator(); ++i) {
            now = i->path().filename().wstring();
            if (find == now)
            {
                listOfFiles.push_back(i->path().wstring());
                wcout << "push" << endl;
            }
                //wcout << i->path().filename().wstring() << endl;
        }
    }
    return listOfFiles;
}
//---start block search-----------------------------------------------------------------------------------------------------------
static void searchDir(const path& where, const path& search, const directory_options& e) 
{
	if (search.has_filename() && !search.has_extension())// recheck serch option
	{
		for (auto& it : directory_iterator(where, e))
		{
			if (it.path().filename() == search.filename())
				$(it);
		}
	}
	else if (search.has_filename() && search.has_extension()) 
	{
		for (auto& it : directory_iterator(where, e))
		{
			if (it.path().filename() == search.filename())
				$(it);
		}
	}
	else if (!search.has_filename() && search.has_extension()) 
	{
		for (auto& it : directory_iterator(where, e))
		{
			if (it.path().extension() == search.extension())
				$(it);
		}
	}
	else { $("\nerr..system failure of passing arguments to a function\n"); }	
}
static void searchDirRec(const path& where, const path& search, const directory_options& e)
{
	if (search.has_filename() && !search.has_extension())// recheck serch option
	{
		for (auto& it : recursive_directory_iterator(where, e))
		{
			if (it.path().filename() == search.filename())
				$(it);
		}
	}
	else if (search.has_filename() && search.has_extension())
	{
		for (auto& it : recursive_directory_iterator(where, e))
		{
			if (it.path().filename() == search.filename())
				$(it);
		}
	}
	else if (!search.has_filename() && search.has_extension())
	{
		for (auto& it : recursive_directory_iterator(where, e))
		{
			if (it.path().extension() == search.extension())
				$(it);
		}
	}
	else { $("\nerr..system failure of passing arguments to a function\n"); }
}
static void searchDirDiskList(const path& search, const directory_options& e)
{
	wstring t;
	path cur_disk;
	for (auto i = logDrive.begin(); i != logDrive.end(); ++i) 
	{
		t = *i; t.push_back(L':'); t.push_back(L'\\');
		$("searching ... disk > "); $(t);
		cur_disk=t;
		searchDirRec(cur_disk, search, e);

	}
}
static void findData(const path& current, const path& search, int curRir_curDisk_allDisk, const directory_options& e)
{
	if (curRir_curDisk_allDisk == 1)//current folder
	{
		searchDir(current, search, e);
	}
	else if (curRir_curDisk_allDisk == 2)//current disk
	{
		searchDirRec(current, search, e);
	}
	else if (curRir_curDisk_allDisk == 3)//disk list
	{
		searchDirDiskList(search, e);
	}
	else { $("bad parameters...\n"); }
}
static void askSearchOption()
{
	path search; search = buf;
	$("what search options ? (dir / disk / all disk)> "); wcout.clear(); std::getline(cin, buf);
	if ("dir" == buf) { findData(point, search, 1, neatly_dir); }
	else if ("disk" == buf) { findData(point, search, 2, neatly_dir); }
	else if ("all disk" == buf) { findData(point, search, 3, neatly_dir); }
	else { $("break operation (rong arguments)...\n"); }
}
//---end block search--------------------------------------------------------------------------------------------------------------

int main(int argc, char* argv[])
{  
	//----------------------------------
    setlocale(LC_ALL, "Russian");   
    setDiskList(logDrive);
    point = current_path().root_name();
	point += "\\";
	//---------------------------------
	//GO-->
    cout << point.string() <<" > ";
    std::getline(cin, buf);
    do 
    {
        if (buf == "dir") 
        {
            for (auto& p : directory_iterator(point,neatly_dir))
                cout <<p.path().string() << '\n';
        }
        else if (buf == "dir all") 
        {
            vector<wstring> found;
            wstring t;
			wstring mask = L".pdf";
            for (auto& p : recursive_directory_iterator(point, neatly_dir))
            {
				if(p.path().filename().extension() == mask)
					wcout << p.path().wstring() << "\n";
            }
			$("all job done!\n");
        }

		else if (buf == "new") 
		{
			path temp = point;
			$("what? (file or folder) > "); wcout.clear();  std::getline(cin, buf);
			if (buf == "folder") 
			{
				$("what? (directory or directories)> "); wcout.clear();  std::getline(cin, buf);
				if (buf == "directory") 
				{
					$("what? (name directory) > "); wcout.clear();  std::getline(cin, buf);
					if (!exists(buf)) { 
						create_directory(buf);
						if (exists(buf))
							$("derictory was created\n");
						else
							$("\nerr bad argument for function...\n");
					}
					else { $("\nthis path exist!\n"); }
				}
				else if (buf == "directorys") 
				{
					$("what? (name directory)>\n"); wcout.clear();  std::getline(cin, buf);
					if (!exists(buf)) {
						create_directories(buf);
						if (exists(buf))
							$("derictories was created\n");
						else
							$("\nerr bad argument for function...\n");
					}
					else { $("\nthis path exist!\n"); }
				}
			}
			else if (buf == "file")
			{
				$("what? (name file) > "); wcout.clear();  std::getline(cin, buf);
				temp /= buf;
				if (!exists(temp)) { wofstream file(temp); if (exists(temp)) { $("\nfile created\n"); } else { $("\nerr system not create file\n"); } }
				else { $("\nFile exist!...\n"); }
			}
		}
		else if (buf == "find")
		{
			$("what? (file, folder or extention)> "); wcout.clear(); std::getline(cin, buf);
			if (buf == "file") { $("what? (name) > "); wcout.clear(); std::getline(cin, buf); askSearchOption(); }
			else if (buf == "folder") { $("what? (name folder) > "); wcout.clear(); std::getline(cin, buf); askSearchOption(); }
			else if (buf == "extension") { $("what? (ext? / .txt / etc...) > "); wcout.clear(); std::getline(cin, buf); askSearchOption(); }
		}
		else if (buf == "rename") 
		{
			path oldname;
			$("what? (name file or folder)> "); wcout.clear();  std::getline(cin, buf);
			{
				if (buf == "file") 
				{
					$("what? (name)> "); wcout.clear();  std::getline(cin, buf);
					oldname = current_path() / buf;
					if (exists(oldname) && oldname.has_extension())
					{
						path newname;
						$("what? (new name)> "); wcout.clear();  std::getline(cin, buf);
						newname = current_path() / buf;
						if (newname.has_extension())
						{
							rename(oldname, newname, err);
						}
						else { $("\nerr bad argument for function\n"); }
					}
					else { $("\nerr bad argument for function\n"); }
				}
				else if (buf == "folder") 
				{
					$("what? (name)> "); wcout.clear();  std::getline(cin, buf);
					oldname = current_path() / buf;
					if (exists(oldname) && !oldname.has_extension())
					{
						path newname;
						$("what? (new name)> "); wcout.clear();  std::getline(cin, buf);
						newname = current_path() / buf;
						if (!newname.has_extension())
						{
							newname / "subdir";
							rename(oldname, newname, err);
							if (exists(newname)) { $("\nfolder renamed\n"); }
							else { $("\nerr system break oeration\n"); }
						}
						else { $("\nerr bad argument for function\n"); }
					}
					else { $("\nerr bad argument for function\n"); }
				}
				
			}
		}
		else if (buf == "del")
		{
			path temp;
			$("what? (file or dir) > "); wcout.clear();  std::getline(cin, buf);
			if (buf == "file")
			{
				$("what? (name) > "); wcout.clear();  std::getline(cin, buf);
				temp = point; temp /= buf;
				if (temp.has_filename() && temp.has_extension() && exists(temp))
				{
					if (remove(temp))
						$("\nfile deleted...\n");
					else
						$("\nerr system break operation\n");
				}
			}
			else if (buf == "folder")
			{
				$("what? (name) > "); wcout.clear();  std::getline(cin, buf);
				temp = point; temp /= buf;
				if (temp.has_filename() && !temp.has_extension() && exists(temp))
				{
					uintmax_t p;
					if (p = remove_all(temp))
					{
						std::wcout << " " << p; $("files or directories deleted...\n");
					}
					else
						$("\nerr system break operation\n");
				}
			}
		}
		else if (buf == "go") { $("where? (folder)> "); wcout.clear();  std::getline(cin, buf); if (exists(buf)) { point = buf; } else { $("use correct syntax, like > foldername, myfolder\\otherfolder\n"); } }
        else if (buf == "exit") { exit(0); }
        else if (buf == "disk info") { $("In SYSTEM are: \n"); showDiskInfo(logDrive); }
		else if (buf == "set disk") { showDiskInfo(logDrive); $("which ? > "); wcout.clear(); std::getline(cin, buf); setDiskRoot(buf, logDrive, point); }
        else if (buf == "?") { $("command key: go, back, new, find, del, disk info, exit.... etc :)\n"); }
        else { cout << point.string() << "> "; $(" ? /use this key for get info command...\n"); }
        cout << point.string() << " > ";
        std::getline(cin, buf);
    } while (true);

}

