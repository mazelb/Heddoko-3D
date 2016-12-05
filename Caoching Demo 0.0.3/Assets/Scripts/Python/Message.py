class message():
    def __init__(self,Mess):
        self.Message = Mess
    def dit_moi_ton_secret(self):
        print self.Message
    def voici_mon_secret(self,Mess):
        self.Message = Mess
        return self.Message
