### **Lỗi GitHub: File Quá 100MB và Cách Xử Lý**  

## **1. Nguyên nhân lỗi**
Bạn bị lỗi do **file quá lớn (trên 100MB)** khi push lên GitHub. GitHub không cho phép file trên 100MB trừ khi dùng **Git LFS**.  

## **2. Cách xử lý khi lỡ commit file quá lớn**  
Bạn cần **xóa file khỏi lịch sử Git** rồi push lại. Làm theo các bước sau:  

### **2.1 Xóa file quá lớn khỏi lịch sử commit**  
Chạy lệnh sau để xóa các file lớn khỏi commit gần nhất:  
```sh
git rm --cached Client/Assets/Firebase/Plugins/x86_64/FirebaseCppApp-12_7_0.bundle
git rm --cached Client/Assets/Firebase/Plugins/x86_64/FirebaseCppApp-12_7_0.so
git commit --amend -C HEAD
```
> `--cached` giúp xóa file khỏi Git nhưng vẫn giữ lại trên máy.  

### **2.2 Dùng `git filter-repo` nếu file đã bị commit từ lâu**  
Nếu file đã tồn tại trong nhiều commit, cần xóa khỏi toàn bộ lịch sử:  
```sh
git filter-repo --path Client/Assets/Firebase/Plugins/x86_64/FirebaseCppApp-12_7_0.bundle --invert-paths
git filter-repo --path Client/Assets/Firebase/Plugins/x86_64/FirebaseCppApp-12_7_0.so --invert-paths
```
> Nếu chưa có `git filter-repo`, cài bằng lệnh:  
> ```sh
> pip install git-filter-repo
> ```

### **2.3 Push lại với `--force` (cẩn thận!)**  
Sau khi xóa file khỏi commit, bạn cần push lại với `--force` để cập nhật repo:  
```sh
git push origin --force
```
> **Cẩn thận**: `--force` có thể ghi đè lịch sử commit, không nên dùng nếu làm việc nhóm.  

---

## **3. Cách tránh lỗi này lần sau**
### **3.1 Dùng `.gitignore` để bỏ qua file lớn**  
Thêm vào `.gitignore`:  
```
/Client/Assets/Firebase/Plugins/x86_64/*.bundle
/Client/Assets/Firebase/Plugins/x86_64/*.so
```

### **3.2 Dùng Git LFS để lưu file lớn**  
Nếu vẫn cần đẩy file lớn lên GitHub, hãy dùng [Git Large File Storage (LFS)](https://git-lfs.github.com/):  
```sh
git lfs install
git lfs track "Client/Assets/Firebase/Plugins/x86_64/*.bundle"
git lfs track "Client/Assets/Firebase/Plugins/x86_64/*.so"
git add .gitattributes
git commit -m "Track large Firebase files with LFS"
git push origin main
```

---

🚀 **Tóm lại**: Nếu file quá lớn, hoặc **xóa khỏi Git**, hoặc dùng **Git LFS** để lưu trên server.

