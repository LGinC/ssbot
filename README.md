# ssbot
通过[go-cqhttp](https://github.com/Mrs4s/go-cqhttp)接收qq机器人发来的消息，并自动调用go-cqhttp api发送消息等

比如做一个涩涩机器人, 发送以!!或者！！开头的群消息，机器人就会开始进行解析，包含涩图则会发一张涩图出来
![image](https://user-images.githubusercontent.com/15363011/192490956-1ea999f9-78e4-4603-b402-73c7cb181ac5.png)


# 部署方式
`docker run -d --name=ssbot -p 7500:80 -v $PWD/setu:/app/images/setu -e Cqhttp__Host=http://192.168.1.100:6700 -e Cqhttp__secret=myqq lginc/ssbot`

其中`-p 7500:80` 是把容器的80端口映射到宿主机的7500端口，请自行替换

`-v $PWD/setu:/app/images/setu` 是把宿主机当前目录下的setu目录映射到容器的/app/images/setu， 可以把$PWD/setu替换为你自己的涩图所在的目录

| 环境变量 | 作用                                  | 示例值                        |
|------|-------------------------------------|----------------------------|
| Cqhttp__Host | cqhttp的服务器地址                        | http://192.168.1.100:6700  |
| Cqhttp__secret | cqhttp的配置中的access-token             | xxxx                       |
| Cqhttp__filter | 需要过滤的命令列表，只要输入的消息在列表内则不做处理，用英文逗号做分割 | 美图,谁在线 |
| ConnectionStrings__redis | redis连接串 | 192.168.1.100,password=123123,connectTimeout=5000,writeBuffer=40960 |
| ChatGPT__ApiKey | chatGPT中申请的apikey | xxxx |
| ChatGPT__Proxy | 发送请求到chatGPT使用的代理，可不加 | http://192.168.1.100:7890 |

使用redis缓存图片列表，避免发送重复的图片(不同的群使用不同的key)
同时也会缓存不同用户发送到chatGPT的消息和结果(只会保留最近10条，缓存2h)，以保证chatGPT能够联系上下文回答

# 修改cqhttp配置
## 修改post上传url
在[cqhttp配置](https://docs.go-cqhttp.org/guide/config.html#%E9%85%8D%E7%BD%AE%E4%BF%A1%E6%81%AF)中有关于post url的配置
![image](https://user-images.githubusercontent.com/15363011/192493254-fba159a7-f721-47d3-bc5f-4e5fef3d706d.png)

这里取消注释(删除行首的#)，  url填入ssbot服务的url，如宿主机是192.168.1.100，端口也是7500，则填写为
```yaml
post:           # 反向HTTP POST地址列表
  - url: 'http://192.168.1.100:7500/recv'                # 地址
```
## 修改默认的filter
filter.json里增加对！！的过滤  \uFF01就是！的unicode
```json
{
    "raw_message": {
        ".regex": "^(!!|\uFF01\uFF01)"
    }
}
```
同时记得把setu目录映射到cqhttp的/data/data/images下，因为cqhttp只能发送这个目录(可以有子目录)下的图片
然后重启cqhttp服务，就可以愉快地涩涩了


# 路线图
+ [x] 发送涩图
+ [x] 识别涩图命令
+ [x] 发送请求到chatGPT(目前仅为GPT-3的api，免费额度$18)
+ [ ] 将cqhttp上报场景抽象为接口，实现对应接口则会自动调用相应的实现类
+ [ ] 分离命令类型识别和对应命令处理方法
