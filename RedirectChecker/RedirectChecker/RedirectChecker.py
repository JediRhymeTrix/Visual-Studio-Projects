import sys
import requests
import time

f1 = open("new.txt",'w')
f1.close()
def check_for_redirects(url):
    try:
        r = requests.get(url, allow_redirects=False, timeout=0.5)
        if 300 <= r.status_code < 400:
            return True
        else:
            return False
    except requests.exceptions.Timeout:
        return False
    except requests.exceptions.ConnectionError:
        return False


def check_domains(url):
    while (True):
        url_to_check = url if url.startswith('http') else "http://%s" % url
        redirect_url = check_for_redirects(url_to_check)
        if redirect_url == True:
            print("True")
            exit(0)
        time.sleep(5)


if __name__ == '__main__':
    fname = 'domains.txt'
    try:
        fname = sys.argv[1]
    except IndexError:
        pass
    url = (open(fname).readline().strip())
    check_domains(url)
